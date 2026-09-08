using NCPF.Pipeline.Application;
using System;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// Base for the MAIN planner — the one pass the agent cannot live without: the path is pure
    /// geometry, neither facing nor speed exist until this pass invents them. A serializable feature
    /// on the planner via [SerializeReference]; the standing one is
    /// <see cref="PlannerStandards"/>.<see cref="PlannerStandards.Main"/>.
    /// </summary>
    [Serializable]
    public abstract class MainPlannerFeature
    {
        public abstract MainSAPass CreatePass(PathAgentContext context);
    }
}
