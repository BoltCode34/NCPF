using NCPF.Domain;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>h = field(x, y, direction) / v_max · eps.</summary>
    public class DijkstraHeuristic3D : IHeuristicHandler
    {
        private readonly IGridMap3D _grid;
        private readonly DijkstraField3D _field;
        private readonly float _eps;

        public DijkstraHeuristic3D(IGridMap3D grid, int target, float eps, float maxSpeed, float turnPenaltyCells = 0.7f)
            : this(grid, target, eps, new DijkstraField3D(grid, target, maxSpeed, turnPenaltyCells)) { }

        public DijkstraHeuristic3D(IGridMap3D grid, int target, float eps, DijkstraField3D field)
        {
            _grid = grid;
            _eps = eps;
            _field = field;
        }

        public DijkstraField3D Field => _field;

        public float Heuristic(int point)
        {
            Config c = _grid.IdToCell3D(point);
            float meters = _field.Lookup(c.X, c.Y, c.Angle);
            if (float.IsInfinity(meters))
                meters = Vector2.Distance(_grid.Cell3DToWorld(c).Position, _field.GoalPos);
            return (meters / _field.MaxSpeed) * _eps;
        }
    }
}
