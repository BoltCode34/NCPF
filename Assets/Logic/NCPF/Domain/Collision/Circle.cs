using System;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>Circular agent shape of the given radius; its bounding square is 2·radius wide.</summary>
    [Serializable]
    public class Circle : Agent
    {
        public float Radius = 0.5f;

        public Circle()
        {
        }

        public Circle(float radius, float cellSize = 1f, Vector2? anchor = null)
            : base(cellSize, anchor)
        {
            Radius = radius;
        }

        public override Vector2 Size => Vector2.one * (Mathf.Max(0f, Radius) * 2f);

        protected internal override bool Covers(Vector2 cellCenter, Vector2 axisX, Vector2 axisY, float halfCell)
            => PointCellDistance(cellCenter, halfCell) <= Mathf.Max(0f, Radius);
    }
}
