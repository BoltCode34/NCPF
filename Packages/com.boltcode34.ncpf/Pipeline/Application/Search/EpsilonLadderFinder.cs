using NCPF.Domain;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// An async finder with the ε ladder. Wraps a plain <see cref="IGoalPathFinder{T}"/> and gives it
    /// a progressively improving search: each rung multiplies the goal's heuristic by eps through
    /// <see cref="EpsGoal"/>; the first success is returned.
    /// </summary>
    public class EpsilonLadderFinder : IAsyncGoalPathFinder<TransitionData>
    {
        private readonly IGoalPathFinder<TransitionData> _finder;
        private readonly float[] _epsilonLadder;
        private readonly int _attemptBudget;

        public EpsilonLadderFinder(IGoalPathFinder<TransitionData> finder, float[] epsilonLadder, int attemptBudget)
        {
            _finder = finder;
            _epsilonLadder = epsilonLadder;
            _attemptBudget = attemptBudget;
        }

        public IPathFindingTask<TransitionData> FindPathAsync(IGraph<TransitionData> graph, int from, IGoal goal, int maxIterations = 1000)
        {
            return new EpsilonLadderTask(_finder, graph, goal, from, _epsilonLadder, _attemptBudget);
        }

        public TransitionData[] FindPath(IGraph<TransitionData> graph, int from, IGoal goal, int maxIterations = 1000)
        {
            return _finder.FindPath(graph, from, goal, maxIterations);
        }
    }
}
