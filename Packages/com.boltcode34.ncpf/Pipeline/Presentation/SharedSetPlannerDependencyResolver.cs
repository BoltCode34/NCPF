using NCPF.Domain;
using NCPF.Pipeline.Application;
using System.Collections.Generic;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// Level B of the cascade: everything that depends on the map AND the control set but on nothing
    /// about the agent — the discretised library, the passability mask and the plan resolver. This is
    /// the expensive tier: the mask alone is a mask-wide bake, so building it once per (map, set)
    /// instead of once per planner is where the startup seconds go.
    /// Groups its planners by what the PRICE depends on: the agent config plus the three penalty
    /// knobs. Same agent, different curvature penalty means different weights, so different graph.
    /// </summary>
    public class SharedSetPlannerDependencyResolver
    {
        private readonly IGridMap3D _map3D;
        private readonly DiscretizedControlSet3D _controlSet;
        private readonly PrimitivePassability _passability;
        private readonly PlanGeometry _geometry;

        private readonly Dictionary<(DynamicAgentConfig, float, float, float),
            SharedAgentPlannerDependencyResolver> _byAgent = new();

        public SharedSetPlannerDependencyResolver(IGridMap3D map3D, NcpfGraphComposer.PlannerConfig config)
        {
            _map3D = map3D;

            ControlSet<IPath> shapes = config.ControlSet.Read(map3D);
            _controlSet = new DiscretizedControlSet3DConverter().Create(map3D, shapes);
            _passability = new PassabilityBaker().Bake(map3D, _controlSet);
            _geometry = new PlanGeometry(_controlSet, map3D);
        }

        public void Resolve(PathPlanner planner, NcpfGraphComposer.PlannerConfig config)
        {
            (DynamicAgentConfig, float, float, float) key = (
                config.DynamicAgent,
                config.CurvaturePenalty,
                config.BaseTimePenalty,
                config.RearGearPenalty);

            if (!_byAgent.TryGetValue(key, out SharedAgentPlannerDependencyResolver resolver))
            {
                resolver = new SharedAgentPlannerDependencyResolver(
                    _map3D, _controlSet, _passability, _geometry, config);
                _byAgent[key] = resolver;
            }

            resolver.Resolve(planner, config);
        }
    }
}
