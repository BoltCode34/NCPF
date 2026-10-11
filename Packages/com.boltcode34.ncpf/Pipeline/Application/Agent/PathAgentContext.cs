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

        public IAsyncGoalPathFinder<TransitionData> AsyncPathFinder;
        public IGoalPathFinder<TransitionData> PathFinder;
        public IGridMap3D Grid;
        public ICurvateGraph Graph;
        public ITarget Target;

        /// <summary>Resolves searched ids back into curves — the search side carries no geometry.</summary>
        public PlanGeometry Geometry;

        public PathAgentContext(PathAgentModel model, DynamicAgent dynamicAgent, IAsyncGoalPathFinder<TransitionData> asyncPathFinder, IGoalPathFinder<TransitionData> pathFinder, IGridMap3D grid, ICurvateGraph graph, ITarget target, PlanGeometry geometry)
        {
            Model = model;
            DynamicAgent = dynamicAgent;
            AsyncPathFinder = asyncPathFinder;
            PathFinder = pathFinder;
            Grid = grid;
            Graph = graph;
            Target = target;
            Geometry = geometry;
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
