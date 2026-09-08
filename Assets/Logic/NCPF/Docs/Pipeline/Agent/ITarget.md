# ITarget

A target that can be followed. It doesn't matter what backs it — a transform, a point, or something else — only the pose is exposed.

`NCPF.Pipeline.Application · Agent/Targets.cs`

## Contract

```csharp
public interface ITarget
{
    Vector2 Position { get; }
    Quaternion Rotation { get; }
    Vector3 Scale { get; }
}
```

Read-only. Who follows a target ([[PathPlanner]], facing passes) reads the pose; they cannot swap the target through this interface. Swapping happens outside, through [[TargetRefHolder]].

## Implementations

- **[[TransformTarget]]** — wrapper over a `Transform` (live scene object).
- **[[PointTarget]]** — a bare point.
- **[[TargetRefHolder]]** — not a target itself, but a holder of a target reference; also implements `ITarget` and proxies the pose through.

## Related

- [[PathPlanner]] — follows the target
- [[TargetRefHolder]] — live swappable reference
- [[TransformTarget]], [[PointTarget]] — implementations