using NCPF.Domain;

namespace NCPF.Bake.Application
{
    /// <summary>
    /// The layer-doubling decorator: decoratee's forward primitives + an own
    /// rear-first run (<see cref="RearFirstLayerBuilder"/>), stored in the layer
    /// interleaved. The runs are INDEPENDENT on purpose — the rear-first pass
    /// keeps its own rings and dedup: the same node pair legitimately owns TWO
    /// curves, nose-first and rear-first; cross-decomposition of a rear-first
    /// primitive through a forward node is deliberately out of the first cut.
    /// </summary>
    public class BackwardLayerBuilder : ILayerControlSetBuilder
    {
        private readonly ILayerControlSetBuilder _decoratee;
        private readonly RearFirstLayerBuilder _backward;

        public BackwardLayerBuilder(ILayerControlSetBuilder decoratee, IPathLimiter limiter, IPathBuilder rearPathBuilder)
        {
            _decoratee = decoratee;
            _backward = new RearFirstLayerBuilder(limiter, rearPathBuilder);
        }

        public IPath[] BuildLayer(IGridGeometry3D gridSpace, int maxCell, WorldConfig origin)
        {
            IPath[] forward = _decoratee.BuildLayer(gridSpace, maxCell, origin);
            IPath[] rearFirst = _backward.BuildLayer(gridSpace, maxCell, origin);

            IPath[] result = new IPath[forward.Length + rearFirst.Length];
            for (int i = 0; i < forward.Length; i++)
            {
                result[i] = forward[i];
            }
            for (int i = 0; i < rearFirst.Length; i++)
            {
                result[forward.Length + i] = rearFirst[i];
            }
            return result;
        }
    }
}
