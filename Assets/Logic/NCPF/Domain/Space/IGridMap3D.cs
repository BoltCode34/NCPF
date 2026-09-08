namespace NCPF.Domain
{
    /// <summary>
    /// The full contract of the 3D lattice the search runs on: lattice
    /// enumeration, world↔cell geometry, id coding and graph adjacency.
    /// </summary>
    public interface IGridMap3D : IGridLattice3D, IGridGeometry3D, IGraphCoder3D, IDirectGraph
    {
    }

    /// <summary>
    /// Geometry of the 3D lattice: cell size, direction layer step, and the
    /// world↔cell transforms that quantise poses onto nodes.
    /// </summary>
    public interface IGridGeometry3D : IGridGeometry2D
    {
        public float AngleStep { get; }
        public WorldConfig Cell3DToWorld(Config config);
        public Config WorldToCell3D(WorldConfig config);
    }
}
