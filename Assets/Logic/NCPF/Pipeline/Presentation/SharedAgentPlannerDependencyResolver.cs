using NCPF.Domain;
using NCPF.Pipeline.Application;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// Level C of the cascade: the graph itself — pricing, gear preference and the neighbour cache —
    /// for one agent configuration. Sharing this is both the memory win and a speed win the planners
    /// could not get separately: agents roaming overlapping ground warm ONE cache, so a node paid
    /// for by the first is free for the rest.
    /// </summary>
    public class SharedAgentPlannerDependencyResolver
    {
        private readonly ICurvateGraph _graph;
        private readonly UniquePlannerDependencyResolver _unique;

        public SharedAgentPlannerDependencyResolver(IGridMap3D map3D,
                                                    DiscretizedControlSet3D controlSet,
                                                    PrimitivePassability passability,
                                                    PlanGeometry geometry,
                                                    NcpfGraphComposer.PlannerConfig config)
        {
            _graph = new CachedCurvateGraph(
                new RearGearGraph(
                    new CurvateMap3D(
                        map3D,
                        new PassabilityTransitionGenerator(
                            controlSet,
                            passability,
                            config.DynamicAgent.Agent,
                            config.CurvaturePenalty,
                            config.BaseTimePenalty)),
                    controlSet,
                    config.RearGearPenalty));

            _unique = new UniquePlannerDependencyResolver(map3D, _graph, geometry);
        }

        public void Resolve(PathPlanner planner, NcpfGraphComposer.PlannerConfig config)
            => _unique.Resolve(planner, config);
    }
}
