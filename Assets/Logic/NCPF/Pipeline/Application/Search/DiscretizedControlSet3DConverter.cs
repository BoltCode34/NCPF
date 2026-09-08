using Core.Foundation.Factories;
using NCPF.Domain;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// Discretizes the geometric control set: walk each shape by ARC LENGTH (0 → Length) and record
    /// every distinct cell it lands in. No time and no speed are involved — an <see cref="IPath"/> is
    /// parameterised by distance, so this is just stepping along the path and writing down the cells.
    /// </summary>
    public class DiscretizedControlSet3DConverter : IFactory<DiscretizedControlSet3D, IGridGeometry3D, ControlSet<IPath>>
    {
        public float Step = 0.05f;

        public DiscretizedControlSet3D Create(IGridGeometry3D space, ControlSet<IPath> controlSet)
        {
            DiscretizedControlSet3D result = new DiscretizedControlSet3D();

            foreach (int layerAngle in controlSet.GetLayers())
            {
                IPath[] paths = controlSet.GetLayerSet(layerAngle);
                if (paths == null || paths.Length == 0)
                    continue;

                DiscretizedPath3D[] discretized = new DiscretizedPath3D[paths.Length];
                for (int i = 0; i < paths.Length; i++)
                    discretized[i] = new DiscretizedPath3D(paths[i], BuildCells(space, paths[i]));

                result.LoadLayer(layerAngle, discretized);
            }

            return result;
        }

        private Config[] BuildCells(IGridGeometry3D space, IPath path)
        {
            float length = path.Length;
            int samples = Mathf.Max(2, Mathf.CeilToInt(length / Step));

            List<Config> list = new List<Config>(samples);

            Config last = default;
            bool hasLast = false;

            for (int i = 0; i <= samples; i++)
            {
                float s = Mathf.Min(i * Step, length);
                Config cell = space.WorldToCell3D(path.Evaluate(s));

                if (!hasLast || cell != last)
                {
                    list.Add(cell);
                    last = cell;
                    hasLast = true;
                }
            }

            return list.ToArray();
        }
    }
}
