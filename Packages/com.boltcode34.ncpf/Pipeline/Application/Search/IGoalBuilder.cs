using System.Threading.Tasks;
using NCPF.Domain;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// The general (async) goal contract: (start, end) → <see cref="IGoal"/> «eventually». A heavy
    /// implementation (a full-map Dijkstra sweep) may build the result off-thread. The finder speaks
    /// THIS contract; swapping the builder sets WHAT is searched without touching the search engine.
    /// </summary>
    public interface IAsyncGoalBuilder
    {
        Task<IGoal> Create(Config start, Config end);
    }

    /// <summary>
    /// The synchronous surface — the degenerate case (value ready immediately): declares a same-named
    /// synchronous Create (hiding the async one) and gives the async member a default that simply
    /// calls the sync one. Cheap goals stay untouched; heavy ones override only the async surface.
    /// </summary>
    public interface IGoalBuilder : IAsyncGoalBuilder
    {
        public new IGoal Create(Config start, Config end);

        Task<IGoal> IAsyncGoalBuilder.Create(Config start, Config end)
            => Task.FromResult(Create(start, end));
    }
}
