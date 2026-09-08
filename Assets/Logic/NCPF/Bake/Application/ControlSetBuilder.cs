using Core.Foundation;
using NCPF.Domain;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Bake.Application
{
    /// <summary>
    /// Bakes a GEOMETRIC control set (Pivtoraiko/Kelly §5.1): node = <see cref="Config"/>
    /// (cell x, y, direction of travel), path = <see cref="IPath"/>, grid =
    /// <see cref="IGridGeometry3D"/>. The bake knows nothing about dynamics — speed is
    /// layered on by the search. Only the first quadrant [0..90°) of start directions
    /// is baked; the remaining quadrants are reconstructed natively on read
    /// (see QuarterSymmetry).
    /// </summary>
    public class ControlSetBuilder : IControlSetBuilder<IPath>
    {
        private readonly ILayerControlSetBuilder _layerBuilder;

        public ControlSetBuilder(ILayerControlSetBuilder layerBuilder)
        {
            _layerBuilder = layerBuilder;
        }

        /// <param name="maxRadius">Safety cap only; the real stop is ring decomposition.</param>
        public ControlSet<IPath> Create(IGridGeometry3D gridSpace, float maxRadius)
        {
            Vector2 zeroPose = gridSpace.CellToWorld(0, 0);
            ControlSet<IPath> controlSet = new ControlSet<IPath>();
            int maxCell = Mathf.CeilToInt(maxRadius / gridSpace.CellSize);
            int angleCount = Mathf.RoundToInt(360f / gridSpace.AngleStep);
            for (int angleIndex = 0; angleIndex < angleCount / 4; angleIndex++)
            {
                WorldConfig origin = new WorldConfig(zeroPose, angleIndex * gridSpace.AngleStep);
                IPath[] set = _layerBuilder.BuildLayer(gridSpace, maxCell, origin);
                controlSet.LoadLayer(angleIndex, set);
            }
            return controlSet;
        }
    }
}
