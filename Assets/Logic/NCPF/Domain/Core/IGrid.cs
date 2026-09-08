using Core.Foundation;

namespace NCPF.Domain
{
    public interface IGrid
    {
        public Int2 Size { get; }
        public bool CellFree(Int2 pos);
        public bool CellFree(int x, int y);

        public int CellToId(int x, int y);
        public int CellToId(Int2 pos);
        public Int2 IdToCell(int id);
    }
}
