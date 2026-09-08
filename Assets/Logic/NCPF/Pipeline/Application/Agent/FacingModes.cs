using NCPF.Domain;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>A "look mode": where the head WANTS to face at a given point of the path. The facing
    /// planner uses this as the objective; obstacles/rate-limit are applied on top.</summary>
    public interface IStateOverride
    {
        DynamicState OverrideState(DynamicState sample);
    }

    public abstract class LookOverride : IStateOverride
    {
        public abstract float OverrideLook(DynamicState sample);

        public DynamicState OverrideState(DynamicState sample)
        {
            return new DynamicState
            (
                sample.X,
                sample.Y,
                OverrideLook(sample),
                sample.VelocityAngle,
                sample.Speed
            );
        }
    }

    /// <summary>Look along the direction of travel (velocity).</summary>
    public class LookForward : LookOverride
    {
        public override float OverrideLook(DynamicState s) => s.VelocityAngle;
    }

    /// <summary>Look at a bare point, which the owner may move at any time (an orbit anchor, a
    /// last-known position — anything that is not an ITarget).</summary>
    public class LookAtPoint : LookOverride
    {
        public Vector2 Position;

        public LookAtPoint(Vector2 position)
        {
            Position = position;
        }

        public override float OverrideLook(DynamicState s)
        {
            Vector2 d = Position - s.Position;
            if (d.sqrMagnitude < 1e-6f) return s.VelocityAngle;
            return Mathf.Atan2(-d.x, d.y) * Mathf.Rad2Deg;
        }
    }

    /// <summary>Look at a target point (the prey). Falls back to travel direction on top of it.</summary>
    public class LookAt : LookOverride
    {
        public ITarget Target;
        public Vector2 TargetPos => Target.Position;
        public LookAt(ITarget target)
        {
            Target = target;
        }

        public override float OverrideLook(DynamicState s)
        {
            Vector2 d = TargetPos - s.Position;
            if (d.sqrMagnitude < 1e-6f) return s.VelocityAngle;
            return Mathf.Atan2(-d.x, d.y) * Mathf.Rad2Deg;
        }
    }
}
