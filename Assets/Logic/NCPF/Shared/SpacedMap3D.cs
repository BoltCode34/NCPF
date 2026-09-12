using Core.Foundation;
using NCPF.Domain;
using UnityEngine;

namespace NCPF.Shared.Presentation
{
    /// <summary>
    /// Adapts a baked <see cref="Map3D"/> to the <see cref="IGridMap3D"/> contract:
    /// world↔cell quantisation, id coding and graph adjacency over the baked C-space.
    /// </summary>
    public class SpacedMap3D : IGridMap3D
    {
        private IGridMap2D _grid;
        private Map3D _map;

        public SpacedMap3D(IGridMap2D grid, Map3D map)
        {
            _grid = grid;
            _map = map;
        }

        public float CellSize => _grid.CellSize;

        public float AngleStep => _map.AngleStep;

        public Int2 Size => _grid.Size;

        public int AngleCount => _map.SizeZ;

        public bool Cell3DFree(Config config) => Cell3DFree(config.X, config.Y, config.Angle);

        /// <summary>
        /// Bounds are checked before id packing: past the edge, CoordToId wraps onto
        /// the opposite side of the map, so out-of-range cells must read as blocked.
        /// </summary>
        public bool Cell3DFree(int x, int y, int angle)
        {
            if (!_map.InBounds(x, y)) return false;
            return _map.PointFree(_map.CoordToId(x, y, angle));
        }

        public int CellToId(int x, int y, int angle)
        {
            return _map.CoordToId(x, y, angle);
        }

        public int Cell3DToId(Config config)
        {
            return _map.CoordToId(config.X, config.Y, config.Angle);
        }

        public DirectTransition[] GetNeightbors(int id)
        {
            return _map.GetNeightbors(id);
        }

        public Config IdToCell3D(int id)
        {
            int x, y, z;
            _map.IdToCoord(id, out x, out y, out z);
            return new Config(x, y, z);
        }

        public bool PointFree(int id)
        {
            return _map.PointFree(id);
        }

        public Vector2 CellToWorld(Int2 cell)=>_grid.CellToWorld(cell);

        public Vector2 CellToWorld(int x, int y)=> _grid.CellToWorld(x, y);

        public WorldConfig IDToWorld(int id)
        {
            int x, y, z;
            _map.IdToCoord(id, out x, out y, out z);
            Vector2 pos = _grid.IDToWorld(_grid.CellToId(x, y));
            return new WorldConfig(pos.x, pos.y, _map.AngleStep * z);
        }

        public Int2 WorldToCell(Vector2 pos) => _grid.WorldToCell(pos);

        /// <summary>
        /// Goes through WorldToCell3D so the angle gets wrapped for any input;
        /// ids built from a raw angle match no lattice node.
        /// </summary>
        public int WorldToID(Vector2 pos, float angle)
        {
            Config cell = WorldToCell3D(new WorldConfig(pos, angle));
            return _map.CoordToId(cell.X, cell.Y, cell.Angle);
        }

        public int WorldToID(WorldConfig config) => WorldToID(new Vector2(config.X, config.Y), config.Angle);

        public WorldConfig Cell3DToWorld(Config config)
        {
            Vector2 pos = _grid.CellToWorld(config.X, config.Y);
            float angle  = config.Angle*AngleStep;
            return new WorldConfig(pos, angle);
        }

        /// <summary>
        /// Wraps the quantised angle into [0, AngleCount) for any input, negative
        /// or past a full turn, so the cell always matches a lattice node.
        /// </summary>
        public Config WorldToCell3D(WorldConfig config)
        {
            Int2 local = _grid.WorldToCell(config.Position);
            int x = local.x;
            int y = local.y;
            int z = Mathf.RoundToInt(config.Angle / _map.AngleStep);
            z = ((z % AngleCount) + AngleCount) % AngleCount;
            return new Config(x, y, z);
        }
    }
}
