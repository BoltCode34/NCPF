# RectangularConfig

A rectangular body shape for an agent. It is the standard [AgentConfig](../../Shared/AgentConfig.md): the map is baked for the shape it describes.

`NCPF.Bake.Presentation` · `RectangularConfig.cs`

## Description

Create one with **Create > NCPF > RectangularConfig**. Assign it to [NCPFMapBuilder](NCPFMapBuilder.md) to bake a map for this shape, and to [PathPlanner](../../Pipeline/Presentation/PathPlanner.md) to draw the body in its preview.

The Inspector shows the shape as it will be baked: a grid of cells with the anchor cell highlighted. See [Bake editors](../Editor/BakeEditors.md).

## Inspector

| Field | Purpose |
|---|---|
| **Size** | Width (`x`) and length (`y`) of the body, in metres. |
| **Orientation** | Currently has no effect. The map bake rotates the body through every heading itself and overwrites this value. Leave it at 0. |
| **Anchor** | The point the body is moved by, as a fraction of the rectangle: `(0, 0)` is the lower-left corner, `(1, 1)` the upper-right, `(0.5, 0.5)` the centre. Use `(0.5, 1)` for a body driven by its front tip. |

## Key API

```csharp
Agent GetAgent(float cellSize)
```

Creates the collision shape the map is baked with, at the given cell size. See [Agent](../../Domain/Collision/Agent.md).

```csharp
Vector2 FootprintSize { get; }
Vector2 FootprintAnchor { get; }
```

The size and anchor, for code that draws the body.

## Notes

- **The anchor must match your agent's pivot.** The planner places the body by its anchor. If the GameObject's pivot is somewhere else, the baked map describes a body that is offset from the one you see, and the agent clips obstacles.
- **Bake the map again after changing Size or Anchor.** Both are baked into the map.

## Related

- [AgentConfig](../../Shared/AgentConfig.md) — the port this implements.
- [Agent](../../Domain/Collision/Agent.md) — the collision shape it creates.
- [NCPFMapBuilder](NCPFMapBuilder.md) — bakes the map for this shape.
