using NCPF.Domain;
using NCPF.Pipeline.Application;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// The tail of the cascade: builds what CANNOT be shared and injects the finished context through
    /// the planner's external-DI seam. Per planner there is live state — the agent model and its
    /// target reference — and the search wrappers, which carry this planner's own ladder and budget.
    /// Everything else in the context arrives already shared from the levels above.
    /// </summary>
    public class UniquePlannerDependencyResolver
    {
        private readonly IGridMap3D _map3D;
        private readonly ICurvateGraph _graph;
        private readonly PlanGeometry _geometry;

        public UniquePlannerDependencyResolver(IGridMap3D map3D, ICurvateGraph graph, PlanGeometry geometry)
        {
            _map3D = map3D;
            _graph = graph;
            _geometry = geometry;
        }

        public void Resolve(PathPlanner planner, NcpfGraphComposer.PlannerConfig config)
        {
            PathAgentContext context = new PathAgentContext(
                new PathAgentModel(),
                config.DynamicAgent.Agent,
                new EpsilonLadderFinder(new CurvateGoalPathFinder(), config.EpsilonLadder, config.AttemptBudget),
                new CurvateGoalPathFinder(),
                _map3D,
                _graph,
                new TargetRefHolder(planner.Prey),
                _geometry);

            planner.Construct(context, config.AttemptBudget);
        }
    }
}
