using NCPF.Pipeline.Application;
using System;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// The DEFAULT heuristic: straight distance / v_max. Cheap, no field. Substituted when no
    /// heuristic feature is authored (null).
    /// </summary>
    [Serializable]
    public class DirectDistHeuristicFeature : HeuristicFeature
    {
        public override IHeuristicBuilder CreateBuilder(PathAgentContext ctx)
            => new DirectDistHeuristicsBuilder(ctx.Grid, ctx.DynamicAgent.MaxVelocity);
    }
}
