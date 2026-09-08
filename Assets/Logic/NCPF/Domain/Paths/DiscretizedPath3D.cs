using System.Collections.Generic;

namespace NCPF.Domain
{
    /// <summary>
    /// A pure geometric shape plus the lattice cells it sweeps. Cells are
    /// <see cref="Config"/> (x, y, direction): there is no velocity anywhere,
    /// in the type or in the data. Collision reads the sweep, the search reads
    /// the geometry, and dynamics are layered on only after the search.
    /// </summary>
    public class DiscretizedPath3D : IPath, IPathWrapper
    {
        public readonly IPath Path;
        public readonly Config[] IdSequence;

        public DiscretizedPath3D(IPath path, Config[] idSequence)
            => (Path, IdSequence) = (path, idSequence);

        public IPath Inner => Path;

        public float Length => Path.Length;

        public WorldConfig Start => Path.Start;

        public WorldConfig End => Path.End;

        public WorldConfig Evaluate(float s) => Path.Evaluate(s);

        public IReadOnlyList<WorldConfig> Sample(float step) => Path.Sample(step);
    }
}
