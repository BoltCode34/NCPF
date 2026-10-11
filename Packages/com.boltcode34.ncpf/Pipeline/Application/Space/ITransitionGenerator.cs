using NCPF.Domain;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// The graph-facing abstraction: a cell in — WEIGHTED transitions out. Primitives are
    /// abstracted away from weight: the implementation runs the set through the projector chain
    /// (projection, validation — existence) and prices the survivors. The weight nonetheless
    /// remains the GRAPH's duty — its decorators rely on exactly the outgoing transition weights —
    /// so the generator is the graph's DELEGATE for weight handling.
    /// </summary>
    public interface ITransitionGenerator
    {
        TransitionData[] GetPrimitives(Config startCell, IGridMap3D grid);
    }
}
