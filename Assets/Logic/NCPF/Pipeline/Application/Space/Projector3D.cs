using NCPF.Domain;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// The pure projector: projects the primitive library, baked at the origin pose, onto the
    /// requested state — and onto it alone. The layered library stores the heading each primitive
    /// starts under, so the projection onto (x, y, θ) takes ONLY layer θ: a pure translation, no
    /// foreign layers, no analysis, no prices. Output is PRIMITIVES, not transitions: no neighbor
    /// id, no weight — assembling or pricing transitions is the edge generator's job.
    /// </summary>
    public class Projector3D : IPrimitivesProjector3D
    {
        private readonly DiscretizedControlSet3D _controlSet;

        public Projector3D(DiscretizedControlSet3D controlSet)
        {
            _controlSet = controlSet;
        }

        public DiscretizedPath3D[] GetPrimitives(Config startCell, IGridMap3D grid)
        {
            List<DiscretizedPath3D> results = new();
            Expand(startCell, startCell.Angle, grid, results);
            return results.ToArray();
        }

        private void Expand(Config startCell, int layer, IGridMap3D grid, List<DiscretizedPath3D> results)
        {
            DiscretizedPath3D[] paths = _controlSet.GetLayerSet(layer);
            if (paths == null) return;

            Vector2 nodePos = grid.Cell3DToWorld(startCell).Position;
            Vector2 origin = grid.Cell3DToWorld(new Config(0, 0, 0)).Position;
            Vector2 shift = nodePos - origin;
            Config cellShift = new Config(startCell.X, startCell.Y, 0);

            for (int i = 0; i < paths.Length; i++)
            {
                DiscretizedPath3D orig = paths[i];
                Config[] cells = OffsetCells(orig.IdSequence, cellShift);
                results.Add(new DiscretizedPath3D(new PathOffset(orig.Path, shift), cells));
            }
        }

        private static Config[] OffsetCells(Config[] source, Config offset)
        {
            Config[] cells = new Config[source.Length];
            for (int i = 0; i < source.Length; i++)
                cells[i] = new Config(source[i].X + offset.X, source[i].Y + offset.Y, source[i].Angle);
            return cells;
        }
    }
}
