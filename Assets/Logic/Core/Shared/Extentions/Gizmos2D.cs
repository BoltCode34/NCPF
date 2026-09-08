using UnityEngine;

namespace Core.Shared.Extentions
{
    public static class Gizmos2D
    {
        public static void DrawWireQuad(Vector3 position, Vector2 size, float angle)
        {
            Vector2 halfSize = size / 2;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector3 topRight = position + rotation * new Vector3(halfSize.x, halfSize.y);
            Vector3 bottomRight = position + rotation * new Vector3(halfSize.x, -halfSize.y);
            Vector3 bottomLeft = position + rotation * new Vector3(-halfSize.x, -halfSize.y);
            Vector3 topLeft = position + rotation * new Vector3(-halfSize.x, halfSize.y);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.DrawLine(topLeft, topRight);

        }

        /// <summary>Квад с ЯКОРЕМ (нормализованным 0..1 внутри размера, конвенция
        /// Rectangle): position — точка ЯКОРЯ, квад рисуется вокруг неё, смещаясь на
        /// (якорь − центр)·size против вращения. Якорь (0.5, 0.5) совпадает с обычной
        /// центрированной отрисовкой.
        /// A quad with a normalized 0..1 ANCHOR inside the size (the Rectangle convention):
        /// position is the ANCHOR point; the quad is drawn around it, offset by
        /// (anchor − center)·size against the rotation. The (0.5, 0.5) anchor degenerates
        /// to the plain centered overload.</summary>
        public static void DrawWireQuad(Vector3 position, Vector2 size, float angle, Vector2 anchor)
        {
            Vector2 offset = new Vector2(anchor.x - 0.5f, anchor.y - 0.5f) * size;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            DrawWireQuad(position - rotation * offset, size, angle);
        }
    }
}
