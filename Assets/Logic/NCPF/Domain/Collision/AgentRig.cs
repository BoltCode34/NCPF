using Core.Foundation;

namespace NCPF.Domain
{
    /// <summary>
    /// The agent's baked footprint snapshot: the boolean grid, its cell size
    /// and the anchor cell at zero heading. Part of the map identity — equal
    /// footprints can still mean different maps when the body is towed by a
    /// different point.
    /// </summary>
    public class AgentRig
    {
        public bool[,] Footprint;
        public int Size => Footprint.GetLength(0);
        public float CellSize;

        public Int2 Anchor;

        public AgentRig(bool[,] footprint, float cellSize, Int2 anchor = default)
        {
            Footprint = footprint;
            CellSize = cellSize;
            Anchor = anchor;
        }
    }
}
