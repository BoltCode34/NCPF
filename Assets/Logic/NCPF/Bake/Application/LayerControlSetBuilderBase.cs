using Core.Foundation;
using NCPF.Domain;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Bake.Application
{
    /// <summary>
    /// Ring-based layer bake: grows concentric rings of target cells around the
    /// origin and connects them with primitives from an <see cref="IPathBuilder"/>,
    /// pruning through an <see cref="IPathLimiter"/>. End headings are only tried
    /// within ±endHeadingSpread steps of the chord bearing — a primitive that
    /// arrives moving hard sideways relative to its travel direction is never a
    /// useful minimal move.
    /// </summary>
    public class LayerControlSetBuilderBase : ILayerControlSetBuilder
    {
        private readonly IPathBuilder _pathBuilder;
        private readonly IPathLimiter _limiter;
        private readonly int _endHeadingSpread;

        private List<Int2> _capturedPos;
        private HashSet<Config> _stateSet;

        public LayerControlSetBuilderBase(IPathLimiter limiter, IPathBuilder pathBuilder, int endHeadingSpread = 2)
        {
            _limiter = limiter;
            _pathBuilder = pathBuilder;
            _endHeadingSpread = Mathf.Max(0, endHeadingSpread);
        }

        /// <summary>
        /// §5.1 termination: grow rings until an ENTIRE ring decomposes (keeps
        /// nothing). maxCell is only a safety cap for pathological setups.
        /// </summary>
        public virtual IPath[] BuildLayer(IGridGeometry3D gridSpace, int maxCell, WorldConfig origin)
        {
            _capturedPos = new List<Int2>();
            _stateSet = new HashSet<Config>();
            List<IPath> set = new List<IPath>();

            for (int radiusIndex = 1; radiusIndex <= maxCell; radiusIndex++)
            {
                float radius = radiusIndex * gridSpace.CellSize;
                IPath[] paths = GenerateRadius(gridSpace, radius, origin);
                set.AddRange(paths);
            }

            return set.ToArray();
        }

        /// <summary>
        /// Two phases per ring: first register the whole ring's nodes, then prune,
        /// so candidates of the same ring can decompose through each other. The
        /// origin cell itself is never a target — no zero-length chord.
        /// </summary>
        protected virtual IPath[] GenerateRadius(IGridGeometry3D gridSpace,
            float radius,
            WorldConfig origin)
        {
            Int2 originCell = gridSpace.WorldToCell(origin.Position);
            List<IPath> paths = new List<IPath>();
            List<(Config cell, IPath path)> candidates = new();
            int cellSize = Mathf.CeilToInt(radius / gridSpace.CellSize);
            for (int x = -cellSize; x <= cellSize; x++)
            {
                for (int y = -cellSize; y <= cellSize; y++)
                {
                    Int2 cell = originCell + new Int2(x, y);
                    if (cell == originCell)
                        continue;
                    Vector2 pos = gridSpace.CellToWorld(cell);
                    if (Vector2.Distance(pos, origin.Position) <= radius && !_capturedPos.Contains(cell))
                    {
                        _capturedPos.Add(cell);
                        var res = GenerateCandidates(gridSpace, pos, origin);
                        candidates.AddRange(res);
                    }
                }
            }
            for (int i = 0; i < candidates.Count; i++)
            {
                _stateSet.Add(candidates[i].cell);
            }
            for (int i = 0; i < candidates.Count; i++)
            {
                GeneratePosition(gridSpace, paths, candidates[i].path, origin);
            }
            return paths.ToArray();
        }

        /// <summary>
        /// Divisibility is re-checked here, now that the candidate's own ring is
        /// in the state set.
        /// </summary>
        protected virtual void GeneratePosition(IGridGeometry3D gridSpace,
            List<IPath> paths,
            IPath path,
            WorldConfig origin)
        {
            if (_limiter.CanBeDivided(gridSpace, _stateSet, path))
            {
                return;
            }
            paths.Add(path);
        }

        /// <summary>
        /// Sweeps end headings around the NCPF bearing of the chord (0=+Y, +CCW,
        /// the exact inverse of AngleUtility.AngleToDirection).
        /// </summary>
        protected virtual (Config cell, IPath path)[] GenerateCandidates(IGridGeometry3D gridSpace,
            Vector2 pos,
            WorldConfig origin)
        {
            List<(Config, IPath)> candidates = new();
            int angleCount = Mathf.RoundToInt(360f / gridSpace.AngleStep);

            Vector2 chord = pos - origin.Position;
            float chordBearing = Mathf.Atan2(-chord.x, chord.y) * Mathf.Rad2Deg;
            int chordIndex = Mathf.RoundToInt(chordBearing / gridSpace.AngleStep);
            int center = HeadingCenter(chordIndex, angleCount);

            for (int k = -_endHeadingSpread; k <= _endHeadingSpread; k++)
            {
                int angleInd = ((center + k) % angleCount + angleCount) % angleCount;
                float angle = gridSpace.AngleStep * angleInd;
                Config cell;
                IPath path = CreatePath(gridSpace, pos, origin, angle, out cell);
                if (!IsSanePrimitive(gridSpace, path, cell))
                {
                    continue;
                }
                if (_limiter.TryAllowPath(path, gridSpace, _stateSet))
                {
                    candidates.Add((cell, path));
                }
            }
            return candidates.ToArray();
        }

        protected virtual IPath CreatePath(IGridGeometry3D gridSpace, Vector2 pos, WorldConfig origin, float endAngle, out Config cell)
        {
            cell = gridSpace.WorldToCell3D(new WorldConfig(pos, endAngle));
            WorldConfig end = gridSpace.Cell3DToWorld(cell);
            return _pathBuilder.Create(origin, end);
        }

        /// <summary>
        /// The centre of the end-heading spread, in channel indices. The base picks
        /// the chord channel: a useful MINIMAL primitive arrives still moving along
        /// the chord. Rear-first travel flips the heading/tangent relation
        /// (heading = tangent − 180°), so its centre is the chord + 180°.
        /// </summary>
        protected virtual int HeadingCenter(int chordIndex, int angleCount) => chordIndex;

        /// <summary>
        /// Rejects broken solver output before it reaches the limiter: the G1 solve
        /// can diverge for near-reversal targets and return garbage lengths whose
        /// endpoint quantises back onto the origin cell. A primitive is sane only
        /// if its length is finite and positive and its ACTUAL endpoint lands
        /// exactly on the node it was built for.
        /// </summary>
        protected virtual bool IsSanePrimitive(IGridGeometry3D gridSpace, IPath path, Config targetCell)
        {
            float length = path.Length;
            if (float.IsNaN(length) || float.IsInfinity(length) || length <= 0f)
                return false;

            WorldConfig end = path.Evaluate(length);
            if (float.IsNaN(end.X) || float.IsNaN(end.Y) || float.IsNaN(end.Angle))
                return false;

            return gridSpace.WorldToCell3D(end) == targetCell;
        }
    }
}
