using NCPF.Domain;
using System.Collections.Generic;

namespace NCPF.Pipeline.Application
{
    /// <summary>Collision decorator: keeps only primitives whose whole cell sweep fits the grid.</summary>
    public class ControlSetValidator3D : IPrimitivesProjector3D
    {
        private readonly IPrimitivesProjector3D _decoratee;

        public ControlSetValidator3D(IPrimitivesProjector3D decoratee)
        {
            _decoratee = decoratee;
        }

        public DiscretizedPath3D[] GetPrimitives(Config startCell, IGridMap3D grid)
        {
            DiscretizedPath3D[] paths = _decoratee.GetPrimitives(startCell, grid);
            List<DiscretizedPath3D> survivors = new List<DiscretizedPath3D>(paths.Length);

            for (int i = 0; i < paths.Length; i++)
                if (CheckPath(paths[i], grid))
                    survivors.Add(paths[i]);

            return survivors.ToArray();
        }

        private bool CheckPath(DiscretizedPath3D path, IGridMap3D grid)
        {
            for (int j = 0; j < path.IdSequence.Length; j++)
                if (!grid.Cell3DFree(path.IdSequence[j]))
                    return false;
            return true;
        }
    }
}
