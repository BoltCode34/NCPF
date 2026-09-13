# DynamicState

The full moving state of a body: its pose, its speed, and the direction it is moving in.

`NCPF.Domain` · `DynamicState.cs`

## Description

A body does not always move where it faces: it can drive in reverse, or look at a target while moving past it. DynamicState keeps the two apart:

- `Pose.Angle` (also `Angle`) is where the body **faces**;
- `VelocityAngle` is the direction it **moves**;
- `Speed` is how fast.

`Speed` is signed on a path: positive when the body drives forward along the current curve, negative when it drives it in reverse. `Velocity` combines speed and direction into a vector.

[PathAgentModel](../Agent/PathAgentModel.md) stores the agent's current DynamicState, and the planner reads it every frame to move the transform.

## Members

| Member | Meaning |
|---|---|
| `Pose` | Position and facing. |
| `Position`, `X`, `Y`, `Angle` | Shortcuts into `Pose`. |
| `VelocityAngle` | The direction of motion, in degrees, same convention as [WorldConfig](WorldConfig.md). |
| `Speed` | The speed, in metres per second. |
| `Velocity` | `Speed` along `VelocityAngle`, as a vector. |
| `VelocityAngleOf(v)` | The NCPF angle of a vector. |

## Notes

- The constructors take their arguments in different orders: `(x, y, angle, velocityAngle, speed)` but `(pose, speed, velocityAngle)`. Check which one you call.

## Related

- [WorldConfig](WorldConfig.md) — the pose.
- [PathAgentModel](../Agent/PathAgentModel.md) — holds the agent's state.
