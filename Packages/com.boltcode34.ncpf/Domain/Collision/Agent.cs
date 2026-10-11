using System;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// A movable body shape for map baking, inscribed in a <see cref="Size"/> bounding rectangle with
    /// its y along the heading. The footprint is an odd NxN boolean grid sized from the bounding
    /// diagonal so it is the same for every rotation; a cell is covered when the shape overlaps it at
    /// all. The anchor is a fraction of the bounding rectangle, rotated with the body. A concrete
    /// shape serializes only its own dimensions and answers only the overlap test; position, heading,
    /// cell size and anchor are bake-time state.
    /// </summary>
    [Serializable]
    public abstract class Agent
    {
        [NonSerialized] public int X = 0;
        [NonSerialized] public int Y = 0;
        [NonSerialized] public float Angle = 0;
        [NonSerialized] public float CellSize = 1;
        [NonSerialized] public Vector2 AnchorFraction = CenterAnchor;

        public static readonly Vector2 CenterAnchor = new Vector2(0.5f, 0.5f);

        protected Agent()
        {
        }

        protected Agent(float cellSize, Vector2? anchor)
        {
            CellSize = cellSize;
            AnchorFraction = anchor ?? CenterAnchor;
        }

        /// <summary>The bounding rectangle in metres: x across the heading, y along it.</summary>
        public abstract Vector2 Size { get; }

        public Int2 Anchor => GetLocalAnchor();
        public bool[,] Footprint => GetFootPrint();

        /// <summary>
        /// Whether the shape, centred at the origin and rotated so that its local axes are
        /// <paramref name="axisX"/> and <paramref name="axisY"/>, overlaps the axis-aligned cell
        /// centred at <paramref name="cellCenter"/> with half size <paramref name="halfCell"/>.
        /// </summary>
        protected internal abstract bool Covers(Vector2 cellCenter, Vector2 axisX, Vector2 axisY, float halfCell);

        public virtual bool[,] GetFootPrint()
        {
            int size = MatrixSize();
            bool[,] footprint = new bool[size, size];

            Axes(out Vector2 axisX, out Vector2 axisY);
            float halfCell = CellSize * 0.5f;
            float indexCenter = (size - 1) * 0.5f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 cellCenter = new Vector2((x - indexCenter) * CellSize, (y - indexCenter) * CellSize);
                    footprint[x, y] = Covers(cellCenter, axisX, axisY, halfCell);
                }
            }

            return footprint;
        }

        /// <summary>The anchor cell, rotated with the body through the same axes as the footprint.</summary>
        public virtual Int2 GetLocalAnchor()
        {
            int size = MatrixSize();
            float indexCenter = (size - 1) * 0.5f;

            Axes(out Vector2 axisX, out Vector2 axisY);
            Vector2 bounds = Size;
            Vector2 offset = axisX * ((AnchorFraction.x - 0.5f) * bounds.x)
                           + axisY * ((AnchorFraction.y - 0.5f) * bounds.y);

            int x = Mathf.RoundToInt(indexCenter + offset.x / CellSize);
            int y = Mathf.RoundToInt(indexCenter + offset.y / CellSize);

            return new Int2(Mathf.Clamp(x, 0, size - 1), Mathf.Clamp(y, 0, size - 1));
        }

        private int MatrixSize()
        {
            int size = Mathf.CeilToInt(Size.magnitude / CellSize) + 1;
            if ((size & 1) == 0)
            {
                size++;
            }
            return size;
        }

        private void Axes(out Vector2 axisX, out Vector2 axisY)
        {
            float angleRad = Angle * Mathf.Deg2Rad;
            axisX = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            axisY = new Vector2(-Mathf.Sin(angleRad), Mathf.Cos(angleRad));
        }

        protected static float PointCellDistance(Vector2 point, float halfCell)
        {
            float dx = Mathf.Max(Mathf.Abs(point.x) - halfCell, 0f);
            float dy = Mathf.Max(Mathf.Abs(point.y) - halfCell, 0f);
            return Mathf.Sqrt(dx * dx + dy * dy);
        }
    }
}
