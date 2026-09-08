using NCPF.Pipeline.Application;
using System;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>Achievement authoring: a feature that creates an <see cref="IAchivementBuilder"/> from the context.</summary>
    [Serializable]
    public abstract class AchivementFeature
    {
        public abstract IAchivementBuilder CreateBuilder(PathAgentContext context);
    }
}
