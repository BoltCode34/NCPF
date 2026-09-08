using NCPF.Domain;
using UnityEngine;

namespace NCPF.Bake.Application
{
    /// <summary>
    /// The rear-first twin of <see cref="ClothoidPathBuilder"/>: the same G1
    /// problem with BOTH tangents flipped by 180° — the body sits on heading θ and
    /// leads with its rear, so the motion tangent is θ+180 at both boundaries.
    /// The solved clothoid ends exactly on the target pose (cell centre) at every
    /// layer, because a 180° rotation is a lattice symmetry always, unlike an
    /// axis reflection which only holds on layers that are multiples of 45°;
    /// <see cref="RearFirstPath"/> takes the 180° back off into the nose-heading
    /// channel.
    /// </summary>
    public sealed class RearFirstPathBuilder : IPathBuilder
    {
        private readonly ClothoidPathBuilder _forward = new ClothoidPathBuilder();

        public IPath Create(WorldConfig start, WorldConfig end)
        {
            WorldConfig tStart = new WorldConfig(start.Position, Mathf.Repeat(start.Angle + 180f, 360f));
            WorldConfig tEnd = new WorldConfig(end.Position, Mathf.Repeat(end.Angle + 180f, 360f));

            return new RearFirstPath((ClothoidPath)_forward.Create(tStart, tEnd));
        }
    }
}
