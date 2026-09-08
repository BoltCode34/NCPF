using System;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>Following target: a live position the planner chases. Read-only surface, swappable from outside.</summary>
    public interface ITarget
    {
        public Vector2 Position { get; }
        public Quaternion Rotation { get; }
        public Vector3 Scale { get; }
    }

    /// <summary>Holds a reference to an ITarget, so the PathPlanner can follow it. The reference can be
    /// changed and other users do not lose it.</summary>
    public class TargetRefHolder : ITarget
    {
        public ITarget Target { get; set; }
        public Vector2 Position => Target.Position;
        public Quaternion Rotation => Target.Rotation;
        public Vector3 Scale => Target.Scale;
        public TargetRefHolder(ITarget target)
        {
            Target = target;
        }
    }

    /// <summary>A bare point as an ITarget: nothing to follow, it just stands there. For states that
    /// aim at a computed spot (an orbit position, a last-known place) rather than at an object.</summary>
    public class PointTarget : ITarget
    {
        public Vector2 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public PointTarget(Vector2 position)
        {
            Position = position;
            Rotation = Quaternion.identity;
            Scale = Vector3.one;
        }
    }

    /// <summary>Wraps a Transform as an ITarget, so the PathPlanner can follow it. The Transform can be
    /// changed from inspector to switch targets.</summary>
    [Serializable]
    public class TransformTarget : ITarget
    {
        [field: SerializeField] public Transform Transform { get; set; }
        public Vector2 Position => Transform.position;
        public Quaternion Rotation => Transform.rotation;
        public Vector3 Scale => Transform.localScale;
        public TransformTarget(Transform target)
        {
            Transform = target;
        }
    }
}
