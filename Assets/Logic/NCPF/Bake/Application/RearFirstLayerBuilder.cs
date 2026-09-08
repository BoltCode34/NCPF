using NCPF.Domain;

namespace NCPF.Bake.Application
{
    /// <summary>
    /// The base builder run "inside-out": same rings/cells/limiters/dedup, but
    /// every primitive is solved by <see cref="RearFirstPathBuilder"/> — G1 with
    /// both tangents +180° — and comes out wrapped in <see cref="RearFirstPath"/>.
    /// The ends land exactly on lattice nodes at EVERY layer, and the
    /// end-heading spread centre shifts by 180°: "arrive still moving along the
    /// chord" reads as chord heading + 180° when travelling rear-first.
    /// </summary>
    public class RearFirstLayerBuilder : LayerControlSetBuilderBase
    {
        public RearFirstLayerBuilder(IPathLimiter limiter, int endHeadingSpread = 2)
            : base(limiter, new RearFirstPathBuilder(), endHeadingSpread)
        {
        }

        protected override int HeadingCenter(int chordIndex, int angleCount)
            => ((chordIndex + angleCount / 2) % angleCount + angleCount) % angleCount;
    }
}
