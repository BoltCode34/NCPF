using System;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>Rectangular agent shape: width and height in metres, height along the heading.</summary>
    [Serializable]
    public class Rectangle : Agent
    {
        public float Width = 1f;
        public float Height = 1f;

        public Rectangle()
        {
        }

        public Rectangle(float width, float height, float cellSize = 1f, Vector2? anchor = null)
            : base(cellSize, anchor)
        {
            Width = width;
            Height = height;
        }

        public override Vector2 Size => new Vector2(Mathf.Max(0f, Width), Mathf.Max(0f, Height));

        protected internal override bool Covers(Vector2 cellCenter, Vector2 axisX, Vector2 axisY, float halfCell)
        {
            float halfW = Mathf.Max(0f, Width) * 0.5f;
            float halfH = Mathf.Max(0f, Height) * 0.5f;

            float obbOnWorldX = Mathf.Abs(axisX.x) * halfW + Mathf.Abs(axisY.x) * halfH;
            float obbOnWorldY = Mathf.Abs(axisX.y) * halfW + Mathf.Abs(axisY.y) * halfH;

            if (Mathf.Abs(cellCenter.x) > halfCell + obbOnWorldX)
            {
                return false;
            }

            if (Mathf.Abs(cellCenter.y) > halfCell + obbOnWorldY)
            {
                return false;
            }

            float cellOnAxisX = halfCell * (Mathf.Abs(axisX.x) + Mathf.Abs(axisX.y));
            float cellOnAxisY = halfCell * (Mathf.Abs(axisY.x) + Mathf.Abs(axisY.y));

            if (Mathf.Abs(Vector2.Dot(cellCenter, axisX)) > halfW + cellOnAxisX)
            {
                return false;
            }

            if (Mathf.Abs(Vector2.Dot(cellCenter, axisY)) > halfH + cellOnAxisY)
            {
                return false;
            }

            return true;
        }
    }
}
