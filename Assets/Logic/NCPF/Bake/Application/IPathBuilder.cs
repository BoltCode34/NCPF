using Core.Foundation.Factories;
using NCPF.Domain;

namespace NCPF.Bake.Application
{
    /// <summary>
    /// Geometric path builder: connects two POSES (position + direction of travel).
    /// This is the bake-side contract — the bake knows nothing about dynamics
    /// (no speed, no time); the search dresses the resulting shapes later.
    /// </summary>
    public interface IPathBuilder : IFactory<IPath, WorldConfig, WorldConfig> { }
}
