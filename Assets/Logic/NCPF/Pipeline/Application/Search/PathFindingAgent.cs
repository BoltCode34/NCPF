using NCPF.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// The GEOMETRIC search: builds a path (async, ε-ladder), buffers it, and on demand stitches it
    /// to the agent's current position. Dynamics-free — pure geometry
    /// (<see cref="CurvateTransition"/>). The goal builds by the async contract
    /// (<see cref="IAsyncGoalBuilder"/> — a heavy heuristic goes off-thread), the search itself runs
    /// in the injected finder (plain A* or rope).
    /// </summary>
    public class PathFindingAgent
    {
        private IGridMap3D _map3D;
        private ICurvateGraph _graph;
        private IAsyncGoalPathFinder<CurvateTransition> _pathFinder;
        private IGoalPathFinder<CurvateTransition> _stitchFinder = new CurvateGoalPathFinder();
        private IAsyncGoalBuilder _goalBuilder;
        private DynamicAgent _dynamicAgent;
        private int _attemptBudget = 400;

        private IPathFindingTask<CurvateTransition> _pathBuildingTask;
        private bool _goalBuilding;
        private int _generation;
        private const int StitchBudget = 200;

        public PathFindingAgent
            (IGridMap3D map3D,
            ICurvateGraph graph,
            IAsyncGoalPathFinder<CurvateTransition> pathFinder,
            IGoalPathFinder<CurvateTransition> stitchFinder,
            IAsyncGoalBuilder goalBuilder,
            DynamicAgent dynamicAgent,
            int attemptBudget)
        {
            _map3D = map3D;
            _graph = graph;
            _pathFinder = pathFinder;
            _stitchFinder = stitchFinder;
            _goalBuilder = goalBuilder;
            _dynamicAgent = dynamicAgent;
            _attemptBudget = attemptBudget;
        }

        /// <summary>Busy with BOTH the goal build and the search itself — the caller's guard sees the whole window.</summary>
        public bool HasTask => _goalBuilding || _pathBuildingTask != null;

        public bool HasBufferedPath => !_goalBuilding
            && _pathBuildingTask != null
            && _pathBuildingTask.Result != null
            && _pathBuildingTask.Result.Length > 0;

        public bool InProcess => _goalBuilding
            || (_pathBuildingTask != null
                && _pathBuildingTask.State == IPathFindingTask<CurvateTransition>.TaskState.InProccess);

        public CurvateTransition[] UnstitchedPath => _pathBuildingTask?.Result;

        /// <summary>
        /// Starts the search. The goal builds asynchronously; the flag is set synchronously BEFORE the
        /// first await, so the caller's InProcess guard sees it immediately — no double sweep. The
        /// continuation lands back on the main thread (Unity context), so the task slot is only ever
        /// mutated on the main thread; no Break() races.
        /// </summary>
        public void BuildPath(DynamicState start, DynamicState end)
        {
            if (_pathFinder == null || _goalBuilding) return;

            Config startCell = _map3D.WorldToCell3D(start.Pose);
            Config endCell = _map3D.WorldToCell3D(end.Pose);
            int startId = _map3D.Cell3DToId(startCell);

            _goalBuilding = true;
            _ = BuildPathAsync(startCell, endCell, startId, _generation);
        }

        private async Task BuildPathAsync(Config startCell, Config endCell, int startId, int generation)
        {
            try
            {
                IGoal goal = await _goalBuilder.Create(startCell, endCell);

                if (generation != _generation) return;

                _pathBuildingTask?.Break();
                _pathBuildingTask = _pathFinder.FindPathAsync(_graph, startId, goal, _attemptBudget);
            }
            catch (Exception)
            {
            }
            finally
            {
                _goalBuilding = false;
            }
        }

        public void Break()
        {
            _generation++;
            if (_pathBuildingTask != null)
            {
                _pathBuildingTask.Break();
                _pathBuildingTask = null;
            }
        }

        /// <summary>
        /// Takes the buffered path and stitches it to the old one jump-free: only the current primitive
        /// of the old path stays (the body is on it), its end is the anchor — a lattice node the body
        /// is guaranteed to pass; then a short sync stitch to the primitive FOLLOWING the one nearest
        /// the anchor on the new path (the nearest itself may already be traveled — stitching to it
        /// would be a detour back), then the tail of the new path from there. Past prefixes are not
        /// carried: the result begins with the bridge (the current primitive), and the cursor resumes
        /// it from its saved arc — an old prefix at the head would be a detour back over traveled
        /// ground. If the stitch fails the result is discarded: raw application of the new path from
        /// its launch point would teleport the body back. The search result is consumed here — the
        /// task is released so it is not stitched twice.
        /// </summary>
        public CurvateTransition[] StitchPath(CurvateTransition[] oldPath, int currentPart)
        {
            if (!HasBufferedPath) return null;

            CurvateTransition[] path = _pathBuildingTask.Result;
            _pathBuildingTask = null;

            if (oldPath == null || oldPath.Length == 0) return path;

            int k = Mathf.Clamp(currentPart, 0, oldPath.Length - 1);
            WorldConfig anchor = oldPath[k].Path.End;

            int j = 0;
            float best = float.MaxValue;
            for (int i = 0; i < path.Length; i++)
            {
                float d = Vector2.Distance(path[i].Path.Start.Position, anchor.Position);
                if (d < best)
                {
                    best = d;
                    j = i;
                }
            }

            int cut = j + 1 < path.Length ? j + 1 : j;
            WorldConfig join = path[cut].Path.Start;

            CurvateTransition[] stitch = _stitchFinder.FindPath(
                _graph,
                _map3D.WorldToID(anchor.Position, anchor.Angle),
                new StitchGoal3D(_map3D, join, _dynamicAgent.MaxVelocity),
                StitchBudget);

            Config anchorCell = _map3D.WorldToCell3D(anchor);
            Config joinCell = _map3D.WorldToCell3D(join);
            if (stitch.Length == 0 && (anchorCell.X != joinCell.X || anchorCell.Y != joinCell.Y))
            {
                return null;
            }

            return new[] { oldPath[k] }.Concat(stitch).Concat(path.Skip(cut)).ToArray();
        }

        /// <summary>
        /// The seam goal: reach the primitive's START CELL on any heading. Not the exact node
        /// (position+heading) — directed primitives may have no route into one precise node, while
        /// the cell is nearly always reachable. Heuristic — straight distance / v_max: the units
        /// match the edge weights, the estimate stays admissible.
        /// </summary>
        private class StitchGoal3D : IGoal
        {
            private readonly IGridMap3D _grid;
            private readonly int _x, _y;
            private readonly Vector2 _target;
            private readonly float _maxSpeed;

            public StitchGoal3D(IGridMap3D grid, WorldConfig target, float maxSpeed)
            {
                _grid = grid;
                Config cell = grid.WorldToCell3D(target);
                _x = cell.X;
                _y = cell.Y;
                _target = target.Position;
                _maxSpeed = Mathf.Max(0.001f, maxSpeed);
            }

            public bool Achieved(int point)
            {
                Config cell = _grid.IdToCell3D(point);
                return cell.X == _x && cell.Y == _y;
            }

            public float Heuristic(int point)
            {
                Vector2 p = _grid.Cell3DToWorld(_grid.IdToCell3D(point)).Position;
                return Vector2.Distance(p, _target) / _maxSpeed;
            }
        }
    }
}
