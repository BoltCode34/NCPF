using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// An agent shape assembled from other shapes: each part is placed by its offset (metres, x across
    /// the heading, y along it) and rotation (degrees, counter-clockwise) inside the composite. The
    /// bounding rectangle is the union of the parts' rotated bounding rectangles, and the anchor is a
    /// fraction of it; a cell is covered when any part covers it.
    /// </summary>
    public class CompositeAgent : Agent
    {
        private readonly Part[] _parts;
        private readonly Vector2 _size;
        private readonly Vector2 _center;

        public CompositeAgent(IReadOnlyList<Part> parts, float cellSize = 1f, Vector2? anchor = null)
            : base(cellSize, anchor)
        {
            List<Part> valid = new List<Part>();
            Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

            if (parts != null)
            {
                foreach (Part part in parts)
                {
                    if (part.Shape == null)
                    {
                        continue;
                    }

                    valid.Add(part);
                    Vector2 half = part.Shape.Size * 0.5f;
                    float rotation = part.Rotation * Mathf.Deg2Rad;
                    float cos = Mathf.Abs(Mathf.Cos(rotation));
                    float sin = Mathf.Abs(Mathf.Sin(rotation));
                    Vector2 extent = new Vector2(cos * half.x + sin * half.y, sin * half.x + cos * half.y);
                    min = Vector2.Min(min, part.Offset - extent);
                    max = Vector2.Max(max, part.Offset + extent);
                }
            }

            _parts = valid.ToArray();
            if (_parts.Length > 0)
            {
                _size = max - min;
                _center = (min + max) * 0.5f;
            }
        }

        public override Vector2 Size => _size;

        protected internal override bool Covers(Vector2 cellCenter, Vector2 axisX, Vector2 axisY, float halfCell)
        {
            for (int i = 0; i < _parts.Length; i++)
            {
                Part part = _parts[i];
                Vector2 local = part.Offset - _center;
                Vector2 partCenter = axisX * local.x + axisY * local.y;

                float rotation = part.Rotation * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rotation);
                float sin = Mathf.Sin(rotation);
                Vector2 partAxisX = axisX * cos + axisY * sin;
                Vector2 partAxisY = axisY * cos - axisX * sin;

                if (part.Shape.Covers(cellCenter - partCenter, partAxisX, partAxisY, halfCell))
                {
                    return true;
                }
            }
            return false;
        }

        public readonly struct Part
        {
            public readonly Agent Shape;
            public readonly Vector2 Offset;
            public readonly float Rotation;

            public Part(Agent shape, Vector2 offset, float rotation)
            {
                Shape = shape;
                Offset = offset;
                Rotation = rotation;
            }
        }
    }
}
