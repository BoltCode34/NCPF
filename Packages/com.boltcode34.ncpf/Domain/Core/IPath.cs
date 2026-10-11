using System.Collections.Generic;

namespace NCPF.Domain
{
    /// <summary>
    /// A pure trajectory: pose (position and direction of travel) as a function of
    /// arc length. Speed and time are not part of the contract, they live in the
    /// pipeline layers.
    /// </summary>
    public interface IPath
    {
        public float Length { get; }

        public WorldConfig Start { get; }
        public WorldConfig End { get; }

        public WorldConfig Evaluate(float s);

        public IReadOnlyList<WorldConfig> Sample(float step);
    }

    /// <summary>
    /// A path whose curvature is the cubic polynomial κ(s) = A·s³ + B·s² + C·s + D.
    /// A clothoid is the degenerate cubic (A = B = 0), so both serialize through
    /// the same (A, B, C, D, L) store.
    /// </summary>
    public interface IPolynomialPath : IPath
    {
        public float A { get; }
        public float B { get; }
        public float C { get; }
        public float D { get; }
    }

    /// <summary>
    /// A path that wraps another path. Lets domain logic (gear detection)
    /// look through decorators that live in higher layers it cannot
    /// reference.
    /// </summary>
    public interface IPathWrapper
    {
        IPath Inner { get; }
    }
}
