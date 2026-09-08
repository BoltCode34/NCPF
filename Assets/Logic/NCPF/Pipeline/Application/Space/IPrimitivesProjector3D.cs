using NCPF.Domain;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// Projects the geometric primitive library onto a 3D search node (x, y, direction). Purely
    /// geometric: no velocity loop, no speed buckets, no centripetal κ-cap. The contract is
    /// DECORATOR-FRIENDLY for EXISTENCE capabilities (collision validation wraps this interface);
    /// ANALYSIS and PRICING belong to the edge generator, never to a projector.
    /// </summary>
    public interface IPrimitivesProjector3D
    {
        DiscretizedPath3D[] GetPrimitives(Config startCell, IGridMap3D grid);
    }
}
