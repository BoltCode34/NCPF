# DynamicAgent

The physical limits of a moving body: top speed, acceleration, and turning rate.

`NCPF.Domain` · `DynamicAgent.cs`

## Description

```csharp
public class DynamicAgent
{
    float MaxVelocity;
    float MaxLinearAcceleration;
    float MaxAngledVelocity;
}
```

These three numbers are all NCPF knows about an agent's body when driving. They decide:

- how fast a curve can be taken: the tighter the turn, the lower the speed, limited both by sideways acceleration and by turning rate;
- how early the agent must brake;
- how fast it can turn to face something.

They also price the search: [PassabilityTransitionGenerator](../../Pipeline/Application/Space/PassabilityTransitionGenerator.md) estimates each curve's travel time from them.

In the Editor, the values come from a [DynamicAgentConfig](../../Pipeline/Presentation/DynamicAgentConfig.md) component.

## Fields

| Field | Unit | Meaning |
|---|---|---|
| `MaxVelocity` | m/s | Top speed. |
| `MaxLinearAcceleration` | m/s² | Used both for speeding up and slowing down, and as the limit on sideways acceleration in turns. |
| `MaxAngledVelocity` | °/s | How fast the body can turn. |

## Constructor

```csharp
DynamicAgent(float acceleration, float maxVelocity, float maxAngledVelocity)
```

Note the order: acceleration comes first.

## Related

- [DynamicAgentConfig](../../Pipeline/Presentation/DynamicAgentConfig.md) — sets these values in the Inspector.
- [CurvatureSpeedProfile](../Agent/CurvatureSpeedProfile.md) — speed limits along a curve.
