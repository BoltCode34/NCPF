using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// Marker: the path is travelled REAR-FIRST. The store flags such entries;
    /// the runtime gear detection tells them apart by the interface without
    /// knowing the concrete class.
    /// </summary>
    public interface IRearFirstPath
    {
    }

    /// <summary>
    /// The natively baked rear-first primitive: the inner clothoid is G1-solved
    /// with BOTH tangents +180° (a body heading θ and leading with its rear has
    /// motion tangent θ+180), so its endpoints land exactly on lattice nodes.
    /// The inner path speaks TANGENTS; the wrapper takes the 180° back off,
    /// restoring the NOSE-heading channel the lattice stores. Positions, length
    /// and curvature pass through untouched — a pure angle-channel shift.
    /// </summary>
    public sealed class RearFirstPath : IPolynomialPath, IRearFirstPath
    {
        private readonly IPolynomialPath _inner;

        public RearFirstPath(IPolynomialPath inner)
        {
            _inner = inner;
        }

        public float Length => _inner.Length;

        public WorldConfig Start => Shift(_inner.Start);

        public WorldConfig End => Shift(_inner.End);

        public float A => _inner.A;
        public float B => _inner.B;
        public float C => _inner.C;
        public float D => _inner.D;

        public WorldConfig Evaluate(float s) => Shift(_inner.Evaluate(s));

        public IReadOnlyList<WorldConfig> Sample(float step)
        {
            IReadOnlyList<WorldConfig> src = _inner.Sample(step);
            WorldConfig[] result = new WorldConfig[src.Count];
            for (int i = 0; i < src.Count; i++)
            {
                result[i] = Shift(src[i]);
            }
            return result;
        }

        private static WorldConfig Shift(WorldConfig pose)
            => new WorldConfig(pose.Position, Mathf.Repeat(pose.Angle - 180f, 360f));
    }
}
