using NCPF.Pipeline.Application;
using System;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>Base for OPTIONAL passes, applied on top of the main planner.</summary>
    [Serializable]
    public abstract class PlannerOverrideFeature
    {
        public abstract SAPass CreatePass(PathAgentContext context);
    }
}
