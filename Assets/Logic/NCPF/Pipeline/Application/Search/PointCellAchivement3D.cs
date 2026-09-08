using Core.Foundation;
using NCPF.Domain;

namespace NCPF.Pipeline.Application
{
    /// <summary>Reached the goal CELL (any direction).</summary>
    public class PointCellAchivement3D : IAchivementHandler
    {
        private readonly IGridMap3D _grid;
        private readonly Int2 _targetCell;

        public PointCellAchivement3D(IGridMap3D grid, int target)
        {
            _grid = grid;
            _targetCell = grid.IdToCell3D(target).Cell;
        }

        public bool Achieved(int point) => _grid.IdToCell3D(point).Cell == _targetCell;
    }
}
