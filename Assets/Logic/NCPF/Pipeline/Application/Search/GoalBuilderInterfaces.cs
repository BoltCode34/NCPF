using NCPF.Domain;
using System.Threading.Tasks;

namespace NCPF.Pipeline.Application
{
    /// <summary>The general (async) heuristic contract: a heavy one (a Dijkstra field) builds off-thread.</summary>
    public interface IAsyncHeuristicBuilder
    {
        Task<IHeuristicHandler> Create(Config start, Config end);
    }

    /// <summary>The general (async) achievement contract.</summary>
    public interface IAsyncAchivementBuilder
    {
        Task<IAchivementHandler> Create(Config start, Config end);
    }

    /// <summary>The synchronous heuristic surface — the degenerate case: a same-named sync Create
    /// hides the async one; the async default simply wraps the sync call.</summary>
    public interface IHeuristicBuilder : IAsyncHeuristicBuilder
    {
        public new IHeuristicHandler Create(Config start, Config end);

        Task<IHeuristicHandler> IAsyncHeuristicBuilder.Create(Config start, Config end)
            => Task.FromResult(Create(start, end));
    }

    /// <summary>The synchronous achievement surface — the degenerate case, mirroring the heuristic.</summary>
    public interface IAchivementBuilder : IAsyncAchivementBuilder
    {
        public new IAchivementHandler Create(Config start, Config end);

        Task<IAchivementHandler> IAsyncAchivementBuilder.Create(Config start, Config end)
            => Task.FromResult(Create(start, end));
    }
}
