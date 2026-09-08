using NCPF.Domain;

namespace NCPF.Pipeline.Application
{
    /// <summary>Cells ↔ world for the AV lattice: buckets ↔ degrees / metres per second.
    /// The AV twin of <see cref="IGridGeometry3D"/>.</summary>
    public interface IGridGeometryAV
    {
        public float AngleStep { get; }
        public float VelocityStep { get; }
        public WorldConfigAV CellAVToWorld(ConfigAV config);
        public ConfigAV WorldToCellAV(WorldConfigAV config);
    }

    /// <summary>Cells ↔ ids and what is admissible, for the AV lattice.
    /// The AV twin of <see cref="IGridLattice3D"/>.</summary>
    public interface IGridLatticeAV : IDirectGraph
    {
        public int SampleCount { get; }
        public int AngleCount { get; }
        public int VelocityCount { get; }

        public bool CellAVFree(ConfigAV config);
        public int CellAVToId(ConfigAV config);
        public ConfigAV IdToCellAV(int id);
    }

    /// <summary>
    /// The (angle, velocity) map — the AV twin of <see cref="IGridMap3D"/>. Same three roles:
    /// a lattice (cells ↔ ids, what is free), a geometry (cells ↔ world units) and a graph
    /// (which moves are physically possible and what they intrinsically cost). It knows nothing
    /// about WHY anyone traverses it; preferences arrive from the outside, as a decorator over
    /// its weights.
    /// </summary>
    public interface IGridMapAV : IGridLatticeAV, IGridGeometryAV
    {
    }
}
