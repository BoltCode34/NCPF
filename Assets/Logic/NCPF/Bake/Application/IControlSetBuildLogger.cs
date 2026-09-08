using NCPF.Domain;

namespace NCPF.Bake.Application
{
    /// <summary>
    /// Geometric bake logger contract: poses and 3D cells only.
    /// </summary>
    public interface IControlSetBuildLogger
    {
        public void StartLogGeneration();
        public void StartLogLayer(WorldConfig origin, Config originCell);
        public void LogPath(Config endCell, WorldConfig end, float radius = -1, float length = -1, bool includedInResult = false);
        public void LogCheckingPath(LimiterPathLog log, Config pos);
        public void MarkLayerAsIncluded(Config end);
        public void SaveLog();
    }
}
