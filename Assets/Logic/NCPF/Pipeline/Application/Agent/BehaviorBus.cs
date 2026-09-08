using NCPF.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// Live behaviour slots the planner swaps without rebuilding the pipeline: the main pass and
    /// the two goal-builder halves sit behind ref decorators, so changing a feature re-points the
    /// decoratee while the search and the follower keep running. Override passes are added/removed
    /// on the follower's pass list. The bus speaks only in app terms — it knows nothing of features.
    /// </summary>
    public class BehaviorBus
    {
        public RefMainPlanner Main { get; } = new RefMainPlanner();
        public RefHeuristicBuilder Heuristic { get; } = new RefHeuristicBuilder();
        public RefAchivementBuilder Achivement { get; } = new RefAchivementBuilder();

        private PathFollowAgent _follower;
        private readonly List<SAPass> _overrides = new List<SAPass>();

        /// <summary>Wires the follower whose pass list the override surface mutates. Call once, before
        /// the first AddOverride; the ref slots are already live and may feed the follower's ctor.</summary>
        public void Attach(PathFollowAgent follower) => _follower = follower;

        public void SetMain(MainSAPass pass) => Main.Decoratee = pass;
        public void SetAchivement(IAchivementBuilder builder) => Achivement.Decoratee = builder;
        public void SetHeuristic(IHeuristicBuilder builder) => Heuristic.Decoratee = builder;

        public void AddOverride(SAPass pass)
        {
            if (pass == null || _overrides.Contains(pass) || _follower == null) return;
            _overrides.Add(pass);
            _follower.Passes.Add(pass);
        }

        public void RemoveOverride(SAPass pass)
        {
            if (pass == null || !_overrides.Contains(pass) || _follower == null) return;
            _overrides.Remove(pass);
            _follower.Passes.Remove(pass);
        }

        public IReadOnlyList<SAPass> Overrides => _overrides;
    }

    /// <summary>Main pass that forwards to a swappable decoratee.</summary>
    public class RefMainPlanner : MainSAPass
    {
        public MainSAPass Decoratee;
        public override ControlInput PlanControl(float dt, DynamicState current) => Decoratee.PlanControl(dt, current);
    }

    /// <summary>Heuristic builder that forwards to a swappable decoratee; the async surface goes
    /// through directly so a heavy heuristic (Dijkstra) never falls back to the sync default.</summary>
    public class RefHeuristicBuilder : IHeuristicBuilder
    {
        public IHeuristicBuilder Decoratee;
        public IHeuristicHandler Create(Config start, Config end) => Decoratee.Create(start, end);

        Task<IHeuristicHandler> IAsyncHeuristicBuilder.Create(Config start, Config end)
            => ((IAsyncHeuristicBuilder)Decoratee).Create(start, end);
    }

    /// <summary>Achivement builder that forwards to a swappable decoratee.</summary>
    public class RefAchivementBuilder : IAchivementBuilder
    {
        public IAchivementBuilder Decoratee;
        public IAchivementHandler Create(Config start, Config end) => Decoratee.Create(start, end);

        Task<IAchivementHandler> IAsyncAchivementBuilder.Create(Config start, Config end)
            => ((IAsyncAchivementBuilder)Decoratee).Create(start, end);
    }
}
