using System;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// Capsule agent shape: a Width-diameter disc swept along the heading so the total length is
    /// Length. A length shorter than the width degenerates into a circle.
    /// </summary>
    [Serializable]
    public class Capsule : Agent
    {
        private const float EPS = 1e-6f;

        public float Width = 0.5f;
        public float Length = 1f;

        public Capsule()
        {
        }

        public Capsule(float width, float length, float cellSize = 1f, Vector2? anchor = null)
            : base(cellSize, anchor)
        {
            Width = width;
            Length = length;
        }

        public override Vector2 Size
        {
            get
            {
                float width = Mathf.Max(0f, Width);
                return new Vector2(width, Mathf.Max(width, Length));
            }
        }

        protected internal override bool Covers(Vector2 cellCenter, Vector2 axisX, Vector2 axisY, float halfCell)
        {
            Vector2 size = Size;
            float radius = size.x * 0.5f;
            float halfSegment = size.y * 0.5f - radius;
            Vector2 start = -axisY * halfSegment - cellCenter;
            Vector2 end = axisY * halfSegment - cellCenter;

            if (SegmentHitsCell(start, end, halfCell))
            {
                return true;
            }

            float distance = Mathf.Min(PointCellDistance(start, halfCell), PointCellDistance(end, halfCell));
            for (int sx = -1; sx <= 1; sx += 2)
            {
                for (int sy = -1; sy <= 1; sy += 2)
                {
                    Vector2 corner = new Vector2(sx * halfCell, sy * halfCell);
                    distance = Mathf.Min(distance, PointSegmentDistance(corner, start, end));
                }
            }

            return distance <= radius;
        }

        private static bool SegmentHitsCell(Vector2 start, Vector2 end, float halfCell)
        {
            Vector2 delta = end - start;
            float enter = 0f;
            float exit = 1f;

            return Clip(-delta.x, start.x + halfCell, ref enter, ref exit)
                && Clip(delta.x, halfCell - start.x, ref enter, ref exit)
                && Clip(-delta.y, start.y + halfCell, ref enter, ref exit)
                && Clip(delta.y, halfCell - start.y, ref enter, ref exit);
        }

        private static bool Clip(float direction, float distance, ref float enter, ref float exit)
        {
            if (Mathf.Abs(direction) < EPS)
            {
                return distance >= 0f;
            }

            float t = distance / direction;
            if (direction < 0f)
            {
                if (t > exit) return false;
                if (t > enter) enter = t;
            }
            else
            {
                if (t < enter) return false;
                if (t < exit) exit = t;
            }
            return true;
        }

        private static float PointSegmentDistance(Vector2 point, Vector2 start, Vector2 end)
        {
            Vector2 delta = end - start;
            float lengthSq = delta.sqrMagnitude;
            float t = lengthSq < EPS ? 0f : Mathf.Clamp01(Vector2.Dot(point - start, delta) / lengthSq);
            return Vector2.Distance(point, start + delta * t);
        }
    }
}
