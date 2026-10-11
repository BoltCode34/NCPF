using NCPF.Domain;

namespace NCPF.Pipeline.Application
{
    public interface IPathFinder3D<T> : IAsyncGoalPathFinder<T> where T : ITransition
    {
        public IPathFindingTask<T> FindPath(IGraph<T> graph, Config from, Config to, int maxIterations = 1000);
    }

    public interface IPathFinder<T> where T : IGraph
    {
        public int[] FindPath(T graph, int from, int to);
    }

    public interface IGoalPathFinder<T> where T : ITransition
    {
        public T[] FindPath(IGraph<T> graph, int from, IGoal goal, int maxIterations = 1000);
    }

    public interface IGraphGoalPathFinder : IGoalPathFinder<ITransition>
    {
    }

    public interface IAchivementHandler
    {
        public bool Achieved(int point);
    }

    public interface IHeuristicHandler
    {
        public float Heuristic(int point);
    }

    /// <summary>
    /// Per-node cost that is ACCUMULATED into g — it actually shapes the returned plan, unlike
    /// <see cref="IHeuristicHandler"/>, which only reorders the search. Use it for a PREFERENCE that
    /// must be paid at every node; it is OPT-IN: a goal that does not implement this costs nothing.
    /// </summary>
    public interface ICostHandler
    {
        public float Cost(int point);
    }

    public interface IGoal : IAchivementHandler, IHeuristicHandler
    {
    }

    public interface ICostedGoal : IGoal, ICostHandler
    {
    }

    public class CostedGoalBase : ICostedGoal
    {
        private readonly IAchivementHandler _achivementHandler;
        private readonly IHeuristicHandler _heuristicHandler;
        private readonly ICostHandler _costHandler;

        public CostedGoalBase(IAchivementHandler achivementHandler, IHeuristicHandler heuristicHandler, ICostHandler costHandler)
        {
            _achivementHandler = achivementHandler;
            _heuristicHandler = heuristicHandler;
            _costHandler = costHandler;
        }

        public bool Achieved(int point) => _achivementHandler.Achieved(point);
        public float Heuristic(int point) => _heuristicHandler.Heuristic(point);
        public float Cost(int point) => _costHandler.Cost(point);
    }

    public class GoalBase : IGoal
    {
        private readonly IAchivementHandler _achivementHandler;
        private readonly IHeuristicHandler _heuristicHandler;

        public GoalBase(IAchivementHandler achivementHandler, IHeuristicHandler heuristicHandler)
        {
            _achivementHandler = achivementHandler;
            _heuristicHandler = heuristicHandler;
        }

        public bool Achieved(int point) => _achivementHandler.Achieved(point);
        public float Heuristic(int point) => _heuristicHandler.Heuristic(point);
    }
}
