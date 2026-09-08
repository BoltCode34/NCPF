using Core.Foundation;

namespace NCPF.Domain
{
    /// <summary>
    /// Enumeration of the 3D lattice: (cell x, cell y, direction) nodes with
    /// free/blocked queries and packed id coding.
    /// </summary>
    public interface IGridLattice3D : IDirectGraph
    {
        public Int2 Size { get; }
        public int AngleCount { get; }
        public bool Cell3DFree(Config config);
        public bool Cell3DFree(int x, int y, int angle);

        public int CellToId(int x, int y, int angle);
        public int Cell3DToId(Config config);
        public Config IdToCell3D(int id);
    }
}
