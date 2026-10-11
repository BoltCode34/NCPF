# LookPass

A facing pass that turns the agent's body toward whatever a look mode asks for, no faster than the agent's turn rate.

`NCPF.Pipeline.Application` · `LookPass.cs`, `FacingModes.cs`

## Description

LookPass changes only the facing. Speed is left exactly as the main planner set it, because the main planner already worked out the timing of the motion at that speed.

What the pass looks at is decided by a **look mode**, an `IStateOverride`. Swapping the look mode is all it takes to aim the same pass at something else, so one pass type covers every kind of looking.

### Look modes

| Mode | Looks at |
|---|---|
| `LookForward` | The direction of travel. |
| `LookAt(ITarget target)` | A target that can move, such as the one the agent chases. |
| `LookAtPoint(Vector2 position)` | A fixed point you can move from code, such as an orbit anchor or a last known position. |

To look somewhere else, derive from `LookOverride` and implement `float OverrideLook(DynamicState sample)`, returning the facing in degrees.

## Constructor

```csharp
LookPass(IStateOverride look, float wMaxDegPerSec)
```

| Parameter | Purpose |
|---|---|
| `look` | Decides where the body should face. |
| `wMaxDegPerSec` | The fastest the body may turn, in degrees per second. |

## Example

A feature that turns the agent toward its target while driving. It does not slow down, plan the turn or check collisions; for that, use [LookAtTargetFeature](../../../Pipeline/Presentation/LookAtTargetFeature.md).

```csharp
[Serializable]
public class LookAtOverrideFeature : PlannerOverrideFeature
{
    public override SAPass CreatePass(PathAgentContext context)
        => new LookPass(new LookAt(context.Target), context.DynamicAgent.MaxAngledVelocity);
}
```

## Related

- [SAPass](SAPass.md) — the pass contract.
- [Planner features](../../../Pipeline/Presentation/PlannerFeatures.md) — adding a pass through the Inspector.
- [ITarget](ITarget.md) — what `LookAt` follows.
