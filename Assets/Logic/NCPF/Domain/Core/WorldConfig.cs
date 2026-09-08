using System;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// A continuous pose: position in metres plus heading in degrees.
    /// </summary>
    [Serializable]
    public struct WorldConfig
    {
        public float X;
        public float Y;
        public float Angle;

        public Vector2 Position => new Vector2(X, Y);

        public WorldConfig(float x, float y, float angle)
        {
            X = x;
            Y = y;
            Angle = angle;
        }

        public WorldConfig(Vector2 pos, float angle)
        {
            X = pos.x;
            Y = pos.y;
            Angle = angle;
        }

        public static WorldConfig operator +(WorldConfig a, WorldConfig b)
            => new WorldConfig(a.X + b.X, a.Y + b.Y, Normalize(a.Angle + b.Angle));

        public static WorldConfig operator -(WorldConfig a, WorldConfig b)
            => new WorldConfig(a.X - b.X, a.Y - b.Y, Normalize(a.Angle - b.Angle));

        public static WorldConfig operator *(WorldConfig a, float k)
            => new WorldConfig(a.X * k, a.Y * k, a.Angle * k);

        public static float Normalize(float angle)
        {
            angle %= 360f;
            if (angle < 0) angle += 360f;
            return angle;
        }

        public static float DeltaAngle(float a, float b)
            => Mathf.DeltaAngle(a, b);

        public override string ToString()
            => $"({X:F2}, {Y:F2}, {Angle:F2})";
    }
}
