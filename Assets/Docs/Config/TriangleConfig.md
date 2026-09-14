# TriangleConfig

An isosceles triangle body shape for an agent, pointing forward.

`NCPF.Config` · `TriangleConfig.cs`

## Description

Create one with **Create > NCPF > TriangleConfig**. It implements the [AgentConfig](../Shared/AgentConfig.md) port and creates a [Triangle](../Domain/Collision/Agent.md#triangle): the base is at the back, the apex at the front.

## Inspector

| Field | Purpose |
|---|---|
| **Width** | The width of the base, in metres. |
| **Height** | The distance from the base to the apex, along the heading, in metres. |
| **Anchor** | The point the body is moved by, as a fraction of the bounding rectangle: `(0.5, 0.5)` is the centre of the rectangle, `(0.5, 1)` the apex, `(0.5, 0)` the middle of the base. |

## Notes

- **The anchor must match your agent's pivot,** and the map must be baked again after changing any field. See [RectangularConfig](RectangularConfig.md#notes).
- `(0.5, 0.5)` is the centre of the bounding rectangle, not of the triangle's area.

## Related

- [AgentConfig](../Shared/AgentConfig.md) — the port and the other shapes.
- [Agent](../Domain/Collision/Agent.md#triangle) — the collision shape it creates.
