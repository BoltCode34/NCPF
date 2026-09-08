using Core.Foundation;
using NCPF.Bake.Presentation;
using NCPF.Domain;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Bake.Presentation
{
    public class SpacedMap2D : IGridMap2D
    {
        private Vector2 _worldSize = new Vector2(10f, 10f);
        private float _cellSize = 1f;

        public Vector2 Anchor;


        private bool[] _cells;

        private Int2 _size;
        public Int2 Size
        {
            get
            {
                var width = Mathf.Max(1, Mathf.CeilToInt(_worldSize.x / CellSize));
                var height = Mathf.Max(1, Mathf.CeilToInt(_worldSize.y / CellSize));
                _size = new Int2(width, height);
                return _size;
            }
        }

        public float CellSize
        {
            get => _cellSize;
            set
            {
                _cellSize = Mathf.Max(0.01f, value);
            }
        }
        public Vector3 CellScale => new Vector3(CellSize, CellSize, 0);
        public SpacedMap2D(Vector2 anchor, Vector2 worldSize, float cellSize)
        {
            Anchor = anchor;
            _worldSize = worldSize;
            _cellSize = cellSize;
            var width = Mathf.Max(1, Mathf.CeilToInt(_worldSize.x / CellSize));
            var height = Mathf.Max(1, Mathf.CeilToInt(_worldSize.y / CellSize));
            _size = new Int2(width, height);
            _cells = new bool[width * height];
        }
        public void SetById(int id, bool value)
        {
            _cells[id] = value;
        }
        public void SetByCoord(int x, int y, bool value)
        {
            int id = CellToId(x, y);
            _cells[id] = value;
        }
        public Vector2 IDToWorld(int id)
        {
            return CellToWorld(IdToCell(id));
        }

        public int WorldToID(Vector2 pos)
        {
            return CellToId(WorldToCell(pos));
        }
        public Vector2 CellToWorld(Int2 cell)
        {
            return Anchor + new Vector2(cell.x * CellSize, cell.y * CellSize);
        }

        public Vector2 CellToWorld(int x, int y)
        {
            return Anchor + new Vector2(x * CellSize, y * CellSize);
        }

        public Int2 WorldToCell(Vector2 worldPosition)
        {
            var local = worldPosition - Anchor;
            var x = Mathf.RoundToInt(local.x / CellSize);
            var y = Mathf.RoundToInt(local.y / CellSize);
            return new Int2(x, y);
        }

        public bool CellFree(Int2 pos)
        {
            return CellFree(pos.x, pos.y);
        }

        public bool CellFree(int x, int y)
        {
            if (!Inside(x, y) || _cells == null)
            {
                return false;
            }

            return _cells[CellToId(x, y)];
        }

        public int CellToId(Int2 cell)
        {
            return CellToId(cell.x, cell.y);
        }

        public int CellToId(int x, int y)
        {
            return y * _size.x + x;
        }

        public Int2 IdToCell(int id)
        {
            var x = id % _size.x;
            var y = id / _size.x;
            return new Int2(x, y);
        }

        private bool Inside(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _size.x && y < _size.y;
        }

    }
}

namespace NCPF.Domain
{
    public class Map2D : ISpacedGraph
    {
        private IGridMap2D _map;

        public Map2D(IGridMap2D map)
        {
            _map = map;
        }

        public DirectTransition[] GetNeightbors(int id)
        {
            Int2 cell = _map.IdToCell(id);
            var neighbors = new List<DirectTransition>(8);
            TryAddStraightNeighbor(cell.x + 1, cell.y, neighbors);
            TryAddStraightNeighbor(cell.x - 1, cell.y, neighbors);
            TryAddStraightNeighbor(cell.x, cell.y + 1, neighbors);
            TryAddStraightNeighbor(cell.x, cell.y - 1, neighbors);

            TryAddDiagonalNeighbor(cell.x + 1, cell.y + 1, cell.x + 1, cell.y, cell.x, cell.y + 1, neighbors);
            TryAddDiagonalNeighbor(cell.x - 1, cell.y + 1, cell.x - 1, cell.y, cell.x, cell.y + 1, neighbors);
            TryAddDiagonalNeighbor(cell.x + 1, cell.y - 1, cell.x + 1, cell.y, cell.x, cell.y - 1, neighbors);
            TryAddDiagonalNeighbor(cell.x - 1, cell.y - 1, cell.x - 1, cell.y, cell.x, cell.y - 1, neighbors);
            return neighbors.ToArray();
        }

        private void TryAddStraightNeighbor(int x, int y, List<DirectTransition> neighbors)
        {
            var id = CellToId(x, y);
            if (!PointFree(id))
            {
                return;
            }

            neighbors.Add(new DirectTransition(id, 1f));
        }
        public int CellToId(int x, int y)=>_map.CellToId(x, y);
        public bool CellFree(int x, int y) => _map.CellFree(x, y);
        private void TryAddDiagonalNeighbor(int targetX, int targetY, int sideX1, int sideY1, int sideX2, int sideY2, List<DirectTransition> neighbors)
        {

            if (!CellFree(targetX, targetY) || !CellFree(sideX1, sideY1) || !CellFree(sideX2, sideY2))
            {
                return;
            }

            var id = CellToId(targetX, targetY);
            neighbors.Add(new DirectTransition(id, 1.41421356f));
        }
        public bool PointFree(int id)
        {
            return _map.CellFree(_map.IdToCell(id));
        }

        public Vector2 IDToWorld(int id)=>_map.IDToWorld(id);

        public int WorldToID(Vector2 pos)=>_map.WorldToID(pos);
    }
}

namespace NCPF.Domain
{
    public class Unity2DGrid : MonoBehaviour, IGridMap2D, ISpacedGraph
    {
        [SerializeField] private bool _drawGizmos;
        [SerializeField] private Vector2 _worldSize = new Vector2(10f, 10f);
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private bool _bakeOnAwake = true;

        private SpacedMap2D _spacedMap2D;
        public SpacedMap2D SpacedMap2D
        {
            get
            {
                if(_spacedMap2D == null)
                {
                    _spacedMap2D = new SpacedMap2D(transform.position, _worldSize, _cellSize);
                    Bake();
                }
                return _spacedMap2D;
            }
        }
        public Map2D _map2D;
        public Map2D Map2D
        {
            get
            {
                if (_map2D == null)
                {
                    _map2D = new Map2D(SpacedMap2D);
                }
                return _map2D;
            }
        }

        public Int2 Size => SpacedMap2D.Size;
        public float CellSize => SpacedMap2D.CellSize;
        public Vector3 CellScale => new Vector3(CellSize, CellSize, 0);
        private void Awake()
        {
            if (_bakeOnAwake)
            {
                Bake();
            }
        }
        public void Bake()
        {
            _spacedMap2D = new SpacedMap2D(transform.position, _worldSize, _cellSize);
            var checkSize = new Vector2(CellSize, CellSize) * 0.9f;
            for (var y = 0; y < SpacedMap2D.Size.y; y++)
            {
                for (var x = 0; x < SpacedMap2D.Size.x; x++)
                {
                    var center = CellToWorld(new Int2(x, y));
                    var blocked = Physics2D.OverlapBox(center, checkSize, 0f, _obstacleMask) == null;
                    SpacedMap2D.SetByCoord(x, y, blocked);
                }
            }
        }
        public Vector2 IDToWorld(int id)=>SpacedMap2D.IDToWorld(id);
        public int WorldToID(Vector2 pos) => SpacedMap2D.WorldToID(pos);
        public Vector2 CellToWorld(Int2 cell)=>SpacedMap2D.CellToWorld(cell);
        public Vector2 CellToWorld(int x, int y)=>SpacedMap2D.CellToWorld(x, y);
        public Int2 WorldToCell(Vector2 worldPosition)=>SpacedMap2D.WorldToCell(worldPosition);
        public bool CellFree(Int2 pos) => SpacedMap2D.CellFree(pos);
        public bool CellFree(int x, int y)=>SpacedMap2D.CellFree(x, y);
        public bool PointFree(int id)=>Map2D.PointFree(id);
        public DirectTransition[] GetNeightbors(int id)=>Map2D.GetNeightbors(id);
        public int CellToId(Int2 cell)=>SpacedMap2D.CellToId(cell);
        public int CellToId(int x, int y)=>SpacedMap2D.CellToId(x, y);
        public Int2 IdToCell(int id)=>SpacedMap2D.IdToCell(id);
        public void OnDrawGizmos()
        {
            if (!_drawGizmos) return;
            Bake();
            for (int x = 0; x < Size.x; x++)
            {
                for(int y = 0; y < Size.y; y++)
                {
                    if(CellFree(x, y))
                    {

                        Color color = Color.cyan;
                        color.a = 0.25f;
                        Gizmos.color = color;
                        Gizmos.DrawWireCube(CellToWorld(x, y), CellScale);
                        color.a = 0.02f;
                        Gizmos.color = color;
                        Gizmos.DrawCube(CellToWorld(x, y), CellScale);
                    }
                    else
                    {
                        Color color = Color.red;
                        color.a = 0.25f;
                        Gizmos.color = color;
                        Gizmos.DrawWireCube(CellToWorld(x, y), CellScale);
                        color.a = 0.02f;
                        Gizmos.color = color;
                        Gizmos.DrawCube(CellToWorld(x, y), CellScale);
                    }

                }

            }
        }

    }
}