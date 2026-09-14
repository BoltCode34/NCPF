# RectangularConfig

A rectangular body shape for an agent. The map is baked for the shape it describes.

`NCPF.Config` · `RectangularConfig.cs`

## Description

Create one with **Create > NCPF > RectangularConfig**. Assign it to [NCPFMapBuilder](../Bake/Presentation/NCPFMapBuilder.md) to bake a map for this shape, and to [PathPlanner](../Pipeline/Presentation/PathPlanner.md) to draw the body in its preview.

It implements the [AgentConfig](../Shared/AgentConfig.md) port and creates a [Rectangle](../Domain/Collision/Agent.md#rectangle). The Inspector shows the shape as it will be baked. See [AgentConfig](../Shared/AgentConfig.md#inspector-preview).

## Inspector

| Field | Purpose |
|---|---|
| **Size** | Width (`x`) and length along the heading (`y`) of the body, in metres. |
| **Anchor** | The point the body is moved by, as a fraction of the rectangle: `(0, 0)` is the lower-left corner, `(1, 1)` the upper-right, `(0.5, 0.5)` the centre. Use `(0.5, 1)` for a body driven by its front tip. |

## Notes

- **The anchor must match your agent's pivot.** The planner places the body by its anchor. If the GameObject's pivot is somewhere else, the baked map describes a body that is offset from the one you see, and the agent clips obstacles.
- **Bake the map again after changing Size or Anchor.** Both are baked into the map.

## Related

- [AgentConfig](../Shared/AgentConfig.md) — the port this implements, and the other shapes.
- [Agent](../Domain/Collision/Agent.md) — the collision shape it creates.
- [NCPFMapBuilder](../Bake/Presentation/NCPFMapBuilder.md) — bakes the map for this shape.
