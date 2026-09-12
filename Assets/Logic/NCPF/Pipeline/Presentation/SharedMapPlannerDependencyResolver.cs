using NCPF.Domain;
using NCPF.Shared.Presentation;
using System.Collections.Generic;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// Level A of the cascade: owns the baked map for one map asset and groups its planners by the
    /// control set. The grid is taken from the first planner of the group — a map asset is baked
    /// against one grid, so every planner holding this map holds that grid too.
    /// </summary>
    public class SharedMapPlannerDependencyResolver
    {
        private readonly IGridMap3D _map3D;
        private readonly Dictionary<ControlSetAsset, SharedSetPlannerDependencyResolver> _bySet = new();

        public SharedMapPlannerDependencyResolver(NcpfGraphComposer.PlannerConfig config)
        {
            _map3D = new SpacedMap3D(config.Grid, config.Map.Read());
        }

        public void Resolve(PathPlanner planner, NcpfGraphComposer.PlannerConfig config)
        {
            if (!_bySet.TryGetValue(config.ControlSet, out SharedSetPlannerDependencyResolver resolver))
            {
                resolver = new SharedSetPlannerDependencyResolver(_map3D, config);
                _bySet[config.ControlSet] = resolver;
            }

            resolver.Resolve(planner, config);
        }
    }
}
