using NCPF.Domain;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// The DEFAULT heuristic: straight distance to the goal divided by v_max. An admissible lower
    /// bound on time (a straight line is never longer than the path), O(1) per node, no field — the
    /// simplest and cheapest. Loses to Dijkstra on accuracy (it sees no walls) but costs nothing.
    /// </summary>
    public class DirectDistHeuristicsBuilder : IHeuristicBuilder
    {
        private readonly IGridMap3D _map3D;
        private readonly float _maxSpeed;

        public DirectDistHeuristicsBuilder(IGridMap3D map3D, float maxSpeed)
        {
            _map3D = map3D;
            _maxSpeed = maxSpeed;
        }

        public IHeuristicHandler Create(Config start, Config end)
        {
            int endId = _map3D.Cell3DToId(end);
            return new DirectDistHeuristic3D(_map3D, endId, _maxSpeed);
        }
    }

    /// <summary>Straight distance / v_max as the time estimate. Units match the edge weight
    /// (length / vMax), so the estimate is admissible and A* stays correct.</summary>
    public class DirectDistHeuristic3D : IHeuristicHandler
    {
        private readonly IGridMap3D _grid;
        private readonly Vector2 _goalPos;
        private readonly float _maxSpeed;

        public DirectDistHeuristic3D(IGridMap3D grid, int endId, float maxSpeed)
        {
            _grid = grid;
            _goalPos = grid.Cell3DToWorld(grid.IdToCell3D(endId)).Position;
            _maxSpeed = Mathf.Max(0.001f, maxSpeed);
        }

        public float Heuristic(int point)
        {
            Vector2 p = _grid.Cell3DToWorld(_grid.IdToCell3D(point)).Position;
            return Vector2.Distance(p, _goalPos) / _maxSpeed;
        }
    }
}
