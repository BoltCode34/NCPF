using NCPF.Domain;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// Live agent dependencies in one place: model, config, map, graph, goal. Behaviours build their
    /// passes on it; the outer code (SuckerResolver) builds its search on it.
    /// </summary>
    public class PathAgentContext
    {
        public PathAgentModel Model;
        public DynamicAgent DynamicAgent;

        public IAsyncGoalPathFinder<CurvateTransition> AsyncPathFinder;
        public IGoalPathFinder<CurvateTransition> PathFinder;
        public IGridMap3D Grid;
        public ICurvateGraph Graph;
        public ITarget Target;

        public PathAgentContext(PathAgentModel model, DynamicAgent dynamicAgent, IAsyncGoalPathFinder<CurvateTransition> asyncPathFinder, IGoalPathFinder<CurvateTransition> pathFinder, IGridMap3D grid, ICurvateGraph graph, ITarget target)
        {
            Model = model;
            DynamicAgent = dynamicAgent;
            AsyncPathFinder = asyncPathFinder;
            PathFinder = pathFinder;
            Grid = grid;
            Graph = graph;
            Target = target;
        }
    }

    public class RefPathAgentContext
    {
        public PathAgentModel Model;
        public DynamicAgent DynamicAgent;
        public IGridMap3D Grid;
        public ICurvateGraph Graph;
    }
}
