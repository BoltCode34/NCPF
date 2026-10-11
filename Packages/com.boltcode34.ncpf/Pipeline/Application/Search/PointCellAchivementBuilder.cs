using NCPF.Domain;

namespace NCPF.Pipeline.Application
{
    /// <summary>The default achievement: reach the prey's cell. Minimal configuration.</summary>
    public class PointCellAchivementBuilder : IAchivementBuilder
    {
        private readonly IGridMap3D _map3D;

        public PointCellAchivementBuilder(IGridMap3D map3D)
        {
            _map3D = map3D;
        }

        public IAchivementHandler Create(Config start, Config end)
        {
            int endId = _map3D.Cell3DToId(end);
            return new PointCellAchivement3D(_map3D, endId);
        }
    }
}
