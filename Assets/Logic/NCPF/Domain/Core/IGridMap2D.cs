
using Core.Foundation;
using UnityEngine;

namespace NCPF.Domain
{
    public interface IGridMap2D : IGrid, IGraphSpace, IGridGeometry2D
    {
    }
    public interface IGridGeometry2D
    {
        public float CellSize { get; }
        public Vector2 CellToWorld(Int2 cell);
        public Vector2 CellToWorld(int x, int y);
        public Int2 WorldToCell(Vector2 pos);

    }
}
