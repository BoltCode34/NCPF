using NCPF.Domain;
using UnityEngine;

namespace NCPF.Bake.Application
{
    /// <summary>
    /// Layer builder decorator for logging layer creation and updates. Purely
    /// geometric, like the pipeline it wraps.
    /// </summary>
    public class LayerBuilderLog : LayerControlSetBuilderBase
    {
        private readonly IControlSetBuildLogger _logger;
        private float _lastRadius;

        public LayerBuilderLog(IControlSetBuildLogger logger,
            IPathLimiter limiter,
            IPathBuilder factory,
            int endHeadingSpread = 2) : base(limiter, factory, endHeadingSpread)
        {
            _logger = logger;
        }

        public override IPath[] BuildLayer(
            IGridGeometry3D gridSpace,
            int maxCell,
            WorldConfig origin)
        {
            _logger.StartLogLayer(origin, gridSpace.WorldToCell3D(origin));
            IPath[] paths = base.BuildLayer(gridSpace, maxCell, origin);
            for (int i = 0; i < paths.Length; i++)
            {
                Config cell = gridSpace.WorldToCell3D(paths[i].Evaluate(paths[i].Length));
                _logger.MarkLayerAsIncluded(cell);
            }
            return paths;
        }

        protected override IPath CreatePath(IGridGeometry3D gridSpace, Vector2 pos, WorldConfig origin, float endAngle, out Config cell)
        {
            IPath path = base.CreatePath(gridSpace, pos, origin, endAngle, out cell);
            _logger.LogPath(cell, path.Evaluate(path.Length), _lastRadius, path.Length);
            return path;
        }

        protected override IPath[] GenerateRadius(IGridGeometry3D gridSpace, float radius, WorldConfig origin)
        {
            _lastRadius = radius;
            return base.GenerateRadius(gridSpace, radius, origin);
        }
    }
}
