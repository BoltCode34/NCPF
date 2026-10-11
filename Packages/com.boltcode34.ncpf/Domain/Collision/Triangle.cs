using System;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// Isosceles triangle agent shape: a Width-wide base at the back of the bounding rectangle and
    /// the apex at the middle of its front edge, Height along the heading.
    /// </summary>
    [Serializable]
    public class Triangle : Agent
    {
        public float Width = 0.5f;
        public float Height = 1f;

        public Triangle()
        {
        }

        public Triangle(float width, float height, float cellSize = 1f, Vector2? anchor = null)
            : base(cellSize, anchor)
        {
            Width = width;
            Height = height;
        }

        public override Vector2 Size => new Vector2(Mathf.Max(0f, Width), Mathf.Max(0f, Height));

        protected internal override bool Covers(Vector2 cellCenter, Vector2 axisX, Vector2 axisY, float halfCell)
        {
            Vector2 size = Size;
            float halfW = size.x * 0.5f;
            float halfH = size.y * 0.5f;

            Vector2 apex = axisY * halfH - cellCenter;
            Vector2 left = -axisX * halfW - axisY * halfH - cellCenter;
            Vector2 right = axisX * halfW - axisY * halfH - cellCenter;

            return !Separated(Vector2.right, apex, left, right, halfCell)
                && !Separated(Vector2.up, apex, left, right, halfCell)
                && !Separated(Normal(apex, left), apex, left, right, halfCell)
                && !Separated(Normal(left, right), apex, left, right, halfCell)
                && !Separated(Normal(right, apex), apex, left, right, halfCell);
        }

        private static Vector2 Normal(Vector2 from, Vector2 to) => new Vector2(from.y - to.y, to.x - from.x);

        private static bool Separated(Vector2 axis, Vector2 a, Vector2 b, Vector2 c, float halfCell)
        {
            float pa = Vector2.Dot(a, axis);
            float pb = Vector2.Dot(b, axis);
            float pc = Vector2.Dot(c, axis);
            float min = Mathf.Min(pa, Mathf.Min(pb, pc));
            float max = Mathf.Max(pa, Mathf.Max(pb, pc));

            float cellRadius = halfCell * (Mathf.Abs(axis.x) + Mathf.Abs(axis.y));
            return min > cellRadius || max < -cellRadius;
        }
    }
}
