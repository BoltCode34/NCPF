# CapsuleConfig

A capsule body shape for an agent: a rectangle with rounded front and back ends.

`NCPF.Config` · `CapsuleConfig.cs`

## Description

Create one with **Create > NCPF > CapsuleConfig**. It implements the [AgentConfig](../Shared/AgentConfig.md) port and creates a [Capsule](../Domain/Collision/Agent.md#capsule).

## Inspector

| Field | Purpose |
|---|---|
| **Width** | The width of the body, which is also the diameter of the rounded ends, in metres. |
| **Length** | The total length along the heading, rounded ends included, in metres. A length shorter than Width gives a circle. |
| **Anchor** | The point the body is moved by, as a fraction of the bounding rectangle: `(0.5, 0.5)` is the centre, `(0.5, 1)` the front point. |

## Notes

- **The anchor must match your agent's pivot,** and the map must be baked again after changing any field. See [RectangularConfig](RectangularConfig.md#notes).

## Related

- [AgentConfig](../Shared/AgentConfig.md) — the port and the other shapes.
- [Agent](../Domain/Collision/Agent.md#capsule) — the collision shape it creates.
