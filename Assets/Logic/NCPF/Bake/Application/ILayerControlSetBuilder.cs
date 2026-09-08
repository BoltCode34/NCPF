using NCPF.Domain;

namespace NCPF.Bake.Application
{
    /// <summary>
    /// Builds one heading layer of the control set over a 3D grid space.
    /// </summary>
    public interface ILayerControlSetBuilder
    {
        public IPath[] BuildLayer(IGridGeometry3D gridSpace, int maxCell, WorldConfig origin);
    }
}
