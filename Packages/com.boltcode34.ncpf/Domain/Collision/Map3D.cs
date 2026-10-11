using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// Occupancy lattice over (x, y, heading): a walkability boolean per cell
    /// per baked direction layer, plus the agent rig the layers were convolved
    /// with. An <see cref="IDirectGraph"/> over packed cell ids.
    /// </summary>
    public class Map3D : IDirectGraph
    {
        public int SizeZ => Map.GetLength(2);
        public readonly float AngleStep;
        public float CellSize;
        public readonly bool[,,] Map;
        public readonly AgentRig Rig;

        public Map3D(bool[,,] maps, AgentRig rig, float angleStep, float cellSize)
        {
            Map = maps;
            AngleStep = angleStep;
            Rig = rig;
            CellSize = cellSize;
        }

        public DirectTransition[] GetNeightbors(int id)
        {
            IdToCoord(id, out int x, out int y, out int z);

            int sizeX = Map.GetLength(0);
            int sizeY = Map.GetLength(1);
            int sizeZ = Map.GetLength(2);

            List<DirectTransition> neighbors = new();

            for (int dz = -1; dz <= 1; dz++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0 && dz == 0)
                            continue;

                        int nx = x + dx;
                        int ny = y + dy;
                        int nz = z + dz;

                        if (nz < 0) nz = sizeZ - 1;
                        if (nz >= sizeZ) nz = 0;

                        if (nx < 0 || ny < 0 ||
                            nx >= sizeX || ny >= sizeY)
                            continue;

                        if (!Map[nx, ny, nz])
                            continue;

                        float weight = Mathf.Sqrt(
                            dx * dx +
                            dy * dy +
                            dz * dz
                        );

                        neighbors.Add(
                            new DirectTransition(
                                CoordToId(nx, ny, nz),
                                weight
                            )
                        );
                    }
                }
            }

            return neighbors.ToArray();
        }

        public bool PointFree(int id)
        {
            IdToCoord(id, out int x, out int y, out int z);
            if (x < 0 || y < 0 || z < 0 ||
                x >= Map.GetLength(0) || y >= Map.GetLength(1) || z >= Map.GetLength(2))
            {
                return false;
            }
            return Map[x, y, z];
        }

        /// <summary>
        /// Is (x, y) actually on the map? Must be asked BEFORE <see cref="CoordToId"/>:
        /// the id packing has no gaps between digits, so an out-of-range coordinate
        /// produces a VALID id of a different cell (x = -1 decodes to the opposite
        /// edge) — by the time <see cref="PointFree"/> unpacks it, x and y are back
        /// in range, just wrong.
        /// </summary>
        public bool InBounds(int x, int y)
            => x >= 0 && y >= 0 && x < Map.GetLength(0) && y < Map.GetLength(1);

        public int CoordToId(int x, int y, int z)
        {
            int sizeX = Map.GetLength(0);
            int sizeY = Map.GetLength(1);

            return z * sizeX * sizeY + x * sizeY + y;
        }

        public void IdToCoord(int id, out int x, out int y, out int z)
        {
            int sizeX = Map.GetLength(0);
            int sizeY = Map.GetLength(1);

            z = id / (sizeX * sizeY);
            int rem = id % (sizeX * sizeY);

            x = rem / sizeY;
            y = rem % sizeY;
        }
    }
}
