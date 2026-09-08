using Core.Foundation;
using NCPF.Domain;
using System.Threading.Tasks;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// Heuristic over a Dijkstra field: an exact lower bound on the time to the goal. Expensive
    /// (sweeps the whole map), so it caches and rebuilds ONLY when the goal cell changes. The instance
    /// contract is one client — one instance — one thread (transient); a builder shared by many
    /// threads is a DIFFERENT type, not a lock inside this one.
    /// </summary>
    public class DijkstraHeuristicBuilder : IHeuristicBuilder
    {
        private readonly IGridMap3D _map3D;
        private readonly float _turnPenaltyCells;
        private readonly float _maxSpeed;

        private DijkstraField3D _dijkstraField;
        private Int2 _fieldGoalCell;
        private bool _hasField;

        public DijkstraHeuristicBuilder(IGridMap3D map3D, float turnPenaltyCells, float maxSpeed)
        {
            _map3D = map3D;
            _turnPenaltyCells = turnPenaltyCells;
            _maxSpeed = maxSpeed;
        }

        public IHeuristicHandler Create(Config start, Config end)
        {
            int endId = _map3D.Cell3DToId(end);
            Int2 goalCell = end.Cell;

            if (!_hasField || _fieldGoalCell != goalCell)
            {
                _dijkstraField = new DijkstraField3D(_map3D, endId, _maxSpeed, _turnPenaltyCells);
                _fieldGoalCell = goalCell;
                _hasField = true;
            }

            return new DijkstraHeuristic3D(_map3D, endId, 1f, _dijkstraField);
        }

        async Task<IHeuristicHandler> IAsyncHeuristicBuilder.Create(Config start, Config end)
        {
            int endId = _map3D.Cell3DToId(end);
            Int2 goalCell = end.Cell;

            if (!_hasField || _fieldGoalCell != goalCell)
            {
                _dijkstraField = await Task.Run(
                    () => new DijkstraField3D(_map3D, endId, _maxSpeed, _turnPenaltyCells));
                _fieldGoalCell = goalCell;
                _hasField = true;
            }

            return new DijkstraHeuristic3D(_map3D, endId, 1f, _dijkstraField);
        }
    }
}
