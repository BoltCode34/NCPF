namespace NCPF.Domain
{
    /// <summary>
    /// The SEARCH-time edge: everything A* reads and nothing else. No shape, no cell sweep — a
    /// primitive is named by its id in the control set, and the set is the only thing that knows how
    /// to turn that back into geometry. Carrying a shape here would mean allocating one per edge per
    /// expansion and discarding it unread: the loop touches only <see cref="Neightbor"/> and
    /// <see cref="Weight"/>, and geometry is needed once, for the plan that won.
    /// <see cref="Start"/> makes a chain of these self-describing — id plus the node it leaves from
    /// is exactly what reconstructing the curve needs.
    /// </summary>
    public struct TransitionData : ITransition
    {
        public int Start;
        public int PrimitiveId;
        public int Neightbor { get; set; }
        public float Weight { get; set; }

        public TransitionData(int start, int neightbor, int primitiveId, float weight)
        {
            Start = start;
            Neightbor = neightbor;
            PrimitiveId = primitiveId;
            Weight = weight;
        }
    }
}
