# CircleConfig

A circular body shape for an agent.

`NCPF.Config` · `CircleConfig.cs`

## Description

Create one with **Create > NCPF > CircleConfig**. It implements the [AgentConfig](../Shared/AgentConfig.md) port and creates a [Circle](../Domain/Collision/Agent.md#circle).

A circle looks the same at every heading, so its map only depends on the heading when the anchor is off-centre.

## Inspector

| Field | Purpose |
|---|---|
| **Radius** | The radius of the body, in metres. |
| **Anchor** | The point the body is moved by, as a fraction of the bounding square: `(0.5, 0.5)` is the centre, `(0.5, 1)` the front point of the circle. |

## Notes

- **The anchor must match your agent's pivot,** and the map must be baked again after changing Radius or Anchor. See [RectangularConfig](RectangularConfig.md#notes).

## Related

- [AgentConfig](../Shared/AgentConfig.md) — the port and the other shapes.
- [Agent](../Domain/Collision/Agent.md#circle) — the collision shape it creates.
