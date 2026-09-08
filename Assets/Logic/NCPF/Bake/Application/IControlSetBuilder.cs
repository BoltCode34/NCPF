using NCPF.Domain;

namespace NCPF.Bake.Application
{
    /// <summary>
    /// Bake-side contract: builds the geometric control set over a 3D grid space
    /// (cell x, y, direction). No dynamics — speed is a search-time concern.
    /// </summary>
    public interface IControlSetBuilder<T> where T : IPath
    {
        public ControlSet<T> Create(IGridGeometry3D gridSpace, float maxRadius);
    }
}
