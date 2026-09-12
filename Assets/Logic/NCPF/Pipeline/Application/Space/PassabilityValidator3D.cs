using NCPF.Domain;
using System.Collections.Generic;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// Collision decorator that READS a pre-baked <see cref="PrimitivePassability"/> instead of
    /// walking every primitive's cell sweep on every expansion. Drop-in replacement for
    /// <see cref="ControlSetValidator3D"/>, which recomputes the same answer thousands of times per
    /// search even though it depends only on the node and the (static) map.
    /// CONTRACT: a primitive is addressed by its POSITION in the decoratee's output, which must
    /// therefore be the layer set in order and unfiltered — exactly what <see cref="Projector3D"/>
    /// emits. Anything that filters or reorders between the projector and this decorator desyncs the
    /// indices and the mask answers about the wrong primitive.
    /// </summary>
    public class PassabilityValidator3D : IPrimitivesProjector3D
    {
        private readonly IPrimitivesProjector3D _decoratee;
        private readonly PrimitivePassability _passability;

        public PassabilityValidator3D(IPrimitivesProjector3D decoratee, PrimitivePassability passability)
        {
            _decoratee = decoratee;
            _passability = passability;
        }

        public DiscretizedPath3D[] GetPrimitives(Config startCell, IGridMap3D grid)
        {
            DiscretizedPath3D[] paths = _decoratee.GetPrimitives(startCell, grid);
            int nodeId = grid.Cell3DToId(startCell);

            List<DiscretizedPath3D> survivors = null;
            for (int i = 0; i < paths.Length; i++)
            {
                if (!_passability.Passable(nodeId, i))
                {
                    continue;
                }

                survivors ??= new List<DiscretizedPath3D>(paths.Length);
                survivors.Add(paths[i]);
            }

            return survivors == null
                ? System.Array.Empty<DiscretizedPath3D>()
                : survivors.ToArray();
        }
    }
}
