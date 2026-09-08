using NCPF.Domain;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>Translates a shape by a world offset, leaving its direction untouched. Carries the
    /// raw library entry (<see cref="Inner"/>) so the edge generator can identify it by reference
    /// and price it — translation leaves curvature intact.</summary>
    public sealed class PathOffset : IPath, IPathWrapper
    {
        private readonly IPath _inner;
        private readonly Vector2 _shift;

        public PathOffset(IPath inner, Vector2 shift)
        {
            _inner = inner;
            _shift = shift;
        }

        public IPath Inner => _inner;

        public float Length => _inner.Length;
        public WorldConfig Start => Shift(_inner.Start);
        public WorldConfig End => Shift(_inner.End);
        public WorldConfig Evaluate(float s) => Shift(_inner.Evaluate(s));

        public IReadOnlyList<WorldConfig> Sample(float step)
        {
            IReadOnlyList<WorldConfig> src = _inner.Sample(step);
            WorldConfig[] result = new WorldConfig[src.Count];
            for (int i = 0; i < src.Count; i++) result[i] = Shift(src[i]);
            return result;
        }

        private WorldConfig Shift(WorldConfig c) => new WorldConfig(c.X + _shift.x, c.Y + _shift.y, c.Angle);
    }
}
