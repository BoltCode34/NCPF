using NCPF.Domain;
using UnityEngine;

namespace NCPF.Bake.Application
{
    /// <summary>
    /// Pure bake logic: assembles the builder chain (limiter → clothoid paths →
    /// rear-first doubling → control set) and runs it over a grid space. No
    /// logging and no Unity objects — the presentation layer feeds it handles
    /// and stores the result through its ports.
    /// </summary>
    public class ControlSetBakeService
    {
        public ControlSet<IPath> Bake(IGridGeometry3D space, float maxRadius, float lengthMultiplier, float limitScale, float minTurnRadius)
        {
            IPathLimiter limiter = new PathLimiterBase(lengthMultiplier, limitScale, minTurnRadius);
            IControlSetBuilder<IPath> builder = CreateControlSetBuilder(limiter);
            return builder.Create(space, maxRadius);
        }

        public IPath[] BakeLayer(IGridGeometry3D space, int angleLayer, float maxRadius, float lengthMultiplier, float limitScale, float minTurnRadius)
        {
            IPathLimiter limiter = new PathLimiterBase(lengthMultiplier, limitScale, minTurnRadius);
            ILayerControlSetBuilder builder = CreateLayerBuilder(limiter);
            return builder.BuildLayer(
                space,
                Mathf.CeilToInt(maxRadius / space.CellSize),
                space.Cell3DToWorld(new NCPF.Domain.Config(0, 0, angleLayer)));
        }

        private IControlSetBuilder<IPath> CreateControlSetBuilder(IPathLimiter limiter)
        {
            ILayerControlSetBuilder layerBuilder = CreateLayerBuilder(limiter);
            return new ControlSetBuilder(layerBuilder);
        }

        private ILayerControlSetBuilder CreateLayerBuilder(IPathLimiter limiter)
        {
            return new BackwardLayerBuilder
            (
                new LayerControlSetBuilderBase
                (
                    limiter,
                    new ClothoidPathBuilder()
                ),
                limiter
            );
        }
    }
}
