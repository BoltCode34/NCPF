using NCPF.Domain;
using System.Collections.Generic;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// Edit-time plan rollout: runs a temporary follower over an already-built geometric plan and
    /// collects the resulting state stream up to completion or a step cap. Pure simulation — no
    /// drawing; the caller renders the stream. Used by the planner's gizmo diagnostics.
    /// </summary>
    public static class PlanRollout
    {
        /// <param name="sim">A fresh follower already started on the plan from a seed state.</param>
        public static List<DynamicState> Run(PathFollowAgent sim, float dt, int maxSteps)
        {
            List<DynamicState> states = new List<DynamicState> { sim.CurrentDynamicState };
            for (int i = 0; i < maxSteps && !sim.Completed; i++)
            {
                states.Add(sim.GetNext(dt));
            }
            return states;
        }
    }
}
