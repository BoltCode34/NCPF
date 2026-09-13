# ITarget

Something a planner chases: a position that may move.

`NCPF.Pipeline.Application` · `Targets.cs`

## Description

```csharp
public interface ITarget
{
    Vector2 Position { get; }
    Quaternion Rotation { get; }
    Vector3 Scale { get; }
}
```

The planner reads the target's position each frame to decide whether its plan still ends close enough and where to plan next.

## Implementations

| Type | Use it for |
|---|---|
| `TransformTarget` | Following a Transform. This is what the **Target** field on [PathPlanner](../../../Pipeline/Presentation/PathPlanner.md) holds. Serializable, so it can be set in the Inspector. |
| `PointTarget` | A fixed point you compute, such as an orbit position or a last known location. Set its `Position` from code. |
| `TargetRefHolder` | A reference to another target that can be swapped. The planner wraps its target in one, so retargeting through `PathPlanner.Target` does not require rebuilding anything that holds the reference. |

## Related

- [PathPlanner](../../../Pipeline/Presentation/PathPlanner.md) — `Target` retargets a running planner.
- [LookPass](LookPass.md) — `LookAt` faces a target.
