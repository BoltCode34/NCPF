using Core.Foundation;
using NCPF.Domain;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// Bakes the <see cref="PrimitivePassability"/> mask over a whole lattice: for every node, which
    /// primitives of its heading layer fit the obstacles. This is the collision half of the
    /// projection — the half that genuinely depends on the node — lifted out of the search and paid
    /// for once. A blocked node is left empty: the search never expands one, so its primitives are
    /// never asked for.
    /// </summary>
    public class PassabilityBaker
    {
        public PrimitivePassability Bake(IGridMap3D grid, DiscretizedControlSet3D controlSet)
        {
            Int2 size = grid.Size;
            int layers = grid.AngleCount;

            int capacity = 0;
            for (int layer = 0; layer < layers; layer++)
            {
                DiscretizedPath3D[] paths = controlSet.GetLayerSet(layer);
                if (paths != null && paths.Length > capacity)
                {
                    capacity = paths.Length;
                }
            }

            int nodeCount = NodeCountOf(grid, size, layers);

            PrimitivePassability passability = new PrimitivePassability(nodeCount, capacity);

            for (int layer = 0; layer < layers; layer++)
            {
                DiscretizedPath3D[] paths = controlSet.GetLayerSet(layer);
                if (paths == null)
                {
                    continue;
                }

                for (int x = 0; x < size.x; x++)
                {
                    for (int y = 0; y < size.y; y++)
                    {
                        if (!grid.Cell3DFree(x, y, layer))
                        {
                            continue;
                        }

                        int nodeId = grid.CellToId(x, y, layer);
                        for (int i = 0; i < paths.Length; i++)
                        {
                            if (SweepFree(paths[i].IdSequence, x, y, grid))
                            {
                                passability.Set(nodeId, i, true);
                            }
                        }
                    }
                }
            }

            return passability;
        }

        /// <summary>
        /// Sized by the largest id the lattice actually hands out, not by multiplying the extents:
        /// <see cref="IGridLattice3D.Size"/> reports the LIVE 2D grid while id packing and bounds
        /// come from the BAKED map, and those two can disagree after the grid is resized in the
        /// inspector. Probing removes the assumption — and the packing formula — from this baker.
        /// </summary>
        private static int NodeCountOf(IGridMap3D grid, Int2 size, int layers)
        {
            int maxId = -1;
            for (int layer = 0; layer < layers; layer++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    for (int y = 0; y < size.y; y++)
                    {
                        int id = grid.CellToId(x, y, layer);
                        if (id > maxId)
                        {
                            maxId = id;
                        }
                    }
                }
            }
            return maxId + 1;
        }

        private static bool SweepFree(Config[] baked, int offsetX, int offsetY, IGridMap3D grid)
        {
            for (int i = 0; i < baked.Length; i++)
            {
                if (!grid.Cell3DFree(baked[i].X + offsetX, baked[i].Y + offsetY, baked[i].Angle))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
