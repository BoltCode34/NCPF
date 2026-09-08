using Core.Foundation;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// Rectangular agent shape: width and height in metres, oriented by
    /// <see cref="Agent.Angle"/>. The footprint is an odd NxN boolean grid
    /// sized from the diagonal so it is the same for every rotation.
    /// </summary>
    public class Rectangle : Agent
    {
        public float Width;
        public float Height;

        /// <summary>
        /// The tow point as a fraction of the rectangle: (0, 0) is the lower
        /// left corner, (1, 1) the upper right, (0.5, 0.5) the centre. Whoever
        /// tows the agent defines it — a needle driven by its tip uses (0.5, 1).
        /// Changing it moves only the point the footprint is laid out from,
        /// never the footprint matrix itself.
        /// </summary>
        public Vector2 Anchor;

        public static readonly Vector2 CenterAnchor = new Vector2(0.5f, 0.5f);

        public Rectangle(float width, float height, float cellSize = 1f, Vector2? anchor = null)
            : base(cellSize)
        {
            Width = width;
            Height = height;
            Anchor = anchor ?? CenterAnchor;
        }

        private int MatrixSize()
        {
            float diagonal = Mathf.Sqrt(Width * Width + Height * Height);
            int size = Mathf.CeilToInt(diagonal / CellSize) + 1;
            if ((size & 1) == 0)
            {
                size++;
            }
            return size;
        }

        /// <summary>
        /// The anchor cell in the footprint matrix, rotated with the body
        /// through the same axes the footprint itself uses — otherwise the
        /// error wanders from layer to layer.
        /// </summary>
        public override Int2 GetLocalAnchor()
        {
            if (Mathf.Approximately(Anchor.x, 0.5f) && Mathf.Approximately(Anchor.y, 0.5f))
            {
                return base.GetLocalAnchor();
            }

            int size = MatrixSize();
            float indexCenter = (size - 1) * 0.5f;

            float angleRad = Angle * Mathf.Deg2Rad;
            Vector2 ux = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            Vector2 uy = new Vector2(-Mathf.Sin(angleRad), Mathf.Cos(angleRad));

            Vector2 offset = ux * ((Anchor.x - 0.5f) * Width)
                           + uy * ((Anchor.y - 0.5f) * Height);

            int x = Mathf.RoundToInt(indexCenter + offset.x / CellSize);
            int y = Mathf.RoundToInt(indexCenter + offset.y / CellSize);

            return new Int2(
                Mathf.Clamp(x, 0, size - 1),
                Mathf.Clamp(y, 0, size - 1));
        }

        public override bool[,] GetFootPrint()
        {
            int size = MatrixSize();

            bool[,] footprint = new bool[size, size];

            float angleRad = Angle * Mathf.Deg2Rad;
            Vector2 ux = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
            Vector2 uy = new Vector2(-Mathf.Sin(angleRad), Mathf.Cos(angleRad));

            float halfW = Width * 0.5f;
            float halfH = Height * 0.5f;
            float halfCell = CellSize * 0.5f;

            float indexCenter = (size - 1) * 0.5f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 cellCenter = new Vector2((x - indexCenter) * CellSize, (y - indexCenter) * CellSize);

                    if (IntersectsOrientedRectWithCell(cellCenter, ux, uy, halfW, halfH, halfCell))
                    {
                        footprint[x, y] = true;
                    }
                }
            }

            return footprint;
        }

        private static bool IntersectsOrientedRectWithCell(
            Vector2 cellCenter,
            Vector2 rectAxisX,
            Vector2 rectAxisY,
            float halfRectW,
            float halfRectH,
            float halfCell)
        {
            float tx = cellCenter.x;
            float ty = cellCenter.y;

            float aabbWorldRadius = halfCell;

            float obbOnWorldX = Mathf.Abs(rectAxisX.x) * halfRectW + Mathf.Abs(rectAxisY.x) * halfRectH;
            float obbOnWorldY = Mathf.Abs(rectAxisX.y) * halfRectW + Mathf.Abs(rectAxisY.y) * halfRectH;

            if (Mathf.Abs(tx) > aabbWorldRadius + obbOnWorldX)
            {
                return false;
            }

            if (Mathf.Abs(ty) > aabbWorldRadius + obbOnWorldY)
            {
                return false;
            }

            float tOnRectX = Vector2.Dot(cellCenter, rectAxisX);
            float tOnRectY = Vector2.Dot(cellCenter, rectAxisY);

            float aabbOnRectX = halfCell * (Mathf.Abs(rectAxisX.x) + Mathf.Abs(rectAxisX.y));
            float aabbOnRectY = halfCell * (Mathf.Abs(rectAxisY.x) + Mathf.Abs(rectAxisY.y));

            if (Mathf.Abs(tOnRectX) > halfRectW + aabbOnRectX)
            {
                return false;
            }

            if (Mathf.Abs(tOnRectY) > halfRectH + aabbOnRectY)
            {
                return false;
            }

            return true;
        }
    }
}
