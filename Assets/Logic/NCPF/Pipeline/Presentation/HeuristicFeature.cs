using NCPF.Pipeline.Application;
using System;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// Heuristic authoring: a serializable feature that creates an <see cref="IHeuristicBuilder"/>
    /// from the context. The builder is built ONCE (it takes start/end itself per search). Plain
    /// [Serializable] — lives in the inspector through [SerializeReference].
    /// </summary>
    [Serializable]
    public abstract class HeuristicFeature
    {
        public abstract IHeuristicBuilder CreateBuilder(PathAgentContext context);
    }
}
