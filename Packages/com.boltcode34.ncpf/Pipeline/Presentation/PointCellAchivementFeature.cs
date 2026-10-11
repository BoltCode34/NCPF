using NCPF.Pipeline.Application;
using System;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>The DEFAULT achievement: reach the prey's cell. No configuration.</summary>
    [Serializable]
    public class PointCellAchivementFeature : AchivementFeature
    {
        public override IAchivementBuilder CreateBuilder(PathAgentContext ctx)
            => new PointCellAchivementBuilder(ctx.Grid);
    }
}
