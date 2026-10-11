using System;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// Full dynamic state of the body: pose, signed speed and the velocity heading.
    /// Velocity is Speed applied along Vector2.up rotated by VelocityAngle.
    /// </summary>
    [Serializable]
    public struct DynamicState
    {
        public WorldConfig Pose;
        internal static DynamicState zero = new DynamicState(0, 0, 0, 0, 0);

        public Vector2 Position => Pose.Position;
        public float X => Pose.X;
        public float Y => Pose.Y;
        public float Angle => Pose.Angle;
        public float VelocityAngle;
        public float Speed;
        public Vector2 Velocity => Quaternion.Euler(0, 0, VelocityAngle) * Vector2.up * Speed;

        public DynamicState(float x, float y, float angle, float velocityAngle, float speed)
        {
            Pose = new WorldConfig(x, y, angle);
            VelocityAngle = velocityAngle;
            Speed = speed;
        }

        public DynamicState(Vector2 position, float angle, float velocityAngle, float speed)
        {
            Pose = new WorldConfig(position.x, position.y, angle);
            VelocityAngle = velocityAngle;
            Speed = speed;
        }

        public DynamicState(WorldConfig pose, float speed, float velocityAngle)
        {
            Pose = pose;
            Speed = speed;
            VelocityAngle = velocityAngle;
        }

        public DynamicState(float x, float y, float angle, Vector2 velocity)
            : this(new WorldConfig(x, y, angle), velocity.magnitude, VelocityAngleOf(velocity)) { }

        public DynamicState(WorldConfig pose, Vector2 velocity)
            : this(pose, velocity.magnitude, VelocityAngleOf(velocity)) { }

        public static float VelocityAngleOf(Vector2 v)
            => v.sqrMagnitude < 1e-12f ? 0f : Mathf.Atan2(-v.x, v.y) * Mathf.Rad2Deg;

        public static DynamicState operator +(DynamicState a, DynamicState b)
            => new DynamicState(a.Pose + b.Pose, a.Speed + b.Speed, a.VelocityAngle + b.VelocityAngle);

        public static DynamicState operator -(DynamicState a, DynamicState b)
            => new DynamicState(a.Pose - b.Pose, a.Speed - b.Speed, a.VelocityAngle - b.VelocityAngle);

        public static DynamicState operator *(DynamicState a, float k)
            => new DynamicState(a.Pose * k, a.Speed * k, a.VelocityAngle * k);

        public override string ToString()
            => $"Pos: {Pose}, VelAng: {VelocityAngle}, Spd: {Speed}";
    }
}
