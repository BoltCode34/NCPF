using NCPF.Shared.Presentation;
using System.Collections.Generic;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// The head of the sharing cascade: hands every planner down a chain that goes from general to
    /// specific, so an artifact is built once per the inputs it actually depends on instead of once
    /// per planner. Grouping is expressed as memoisation by key — planners with equal keys meet the
    /// same resolver and therefore the same artifacts, which also means a planner spawned long after
    /// scene load joins its group by simply being resolved.
    /// Level A (here): the baked map, keyed by the map asset.
    /// </summary>
    public class MulPlannerDependencyResolver
    {
        private readonly Dictionary<MapAsset, SharedMapPlannerDependencyResolver> _byMap = new();

        public void ResolveAll(IReadOnlyList<PathPlanner> planners)
        {
            for (int i = 0; i < planners.Count; i++)
            {
                Resolve(planners[i]);
            }
        }

        public void Resolve(PathPlanner planner)
        {
            if (planner == null)
            {
                return;
            }

            NcpfGraphComposer.PlannerConfig config = planner.Config;
            if (config.Grid == null || config.Map == null
                || config.ControlSet == null || config.DynamicAgent == null)
            {
                return;
            }

            if (!_byMap.TryGetValue(config.Map, out SharedMapPlannerDependencyResolver resolver))
            {
                resolver = new SharedMapPlannerDependencyResolver(config);
                _byMap[config.Map] = resolver;
            }

            resolver.Resolve(planner, config);
        }
    }
}
