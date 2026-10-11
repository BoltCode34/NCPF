using System;
using System.Collections.Generic;
using ClothoidX;
using UnityEngine;
using NumVector3 = System.Numerics.Vector3;

namespace NCPF.Domain
{
    /// <summary>
    /// Clothoid (Euler spiral) between two poses, solved as a G1 Hermite
    /// interpolation (Bertolazzi-Frego): both boundary positions and directions
    /// are matched exactly. Pure shape — no speed, no time. Serialises through
    /// <see cref="IPolynomialPath"/> as the degenerate cubic (A = B = 0).
    /// </summary>
    public sealed class ClothoidPath : IPolynomialPath
    {
        private readonly ClothoidSegment _segment;
        private readonly WorldConfig _start;
        private readonly WorldConfig _end;

        public float Length { get; }
        public WorldConfig Start => _start;
        public WorldConfig End => _end;

        public float A => 0;
        public float B => 0;
        public float C { get; }
        public float D { get; }

        private const float ClothoidToNcpfDeg = -90f;

        public ClothoidPath(ClothoidSegment segment, WorldConfig start, WorldConfig end)
        {
            _segment = segment ?? throw new ArgumentNullException(nameof(segment));
            _start = start;
            _end = end;

            Length = (float)_segment.TotalArcLength;
            C = (float)_segment.Sharpness;
            D = (float)_segment.StartCurvature;
        }

        public WorldConfig Evaluate(float s)
        {
            s = Mathf.Clamp(s, 0f, Length);

            NumVector3 p = _segment.GetSample(s);
            var position = new Vector2(p.X, p.Z);

            float angleDeg =
                (float)((_segment.AngleStart + _segment.Tangent(s)) * Mathf.Rad2Deg)
                + ClothoidToNcpfDeg;

            return new WorldConfig(position, angleDeg);
        }

        public IReadOnlyList<WorldConfig> Sample(float step)
        {
            var list = new List<WorldConfig>();

            if (Length <= 0f || step <= 0f)
            {
                list.Add(Start);
                list.Add(End);
                return list;
            }

            for (float s = 0f; s < Length; s += step)
                list.Add(Evaluate(s));

            list.Add(End);
            return list;
        }
    }
}
