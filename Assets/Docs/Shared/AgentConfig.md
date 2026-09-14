# AgentConfig

The port for an agent's body shape. The map is baked for the shape an AgentConfig describes.

`NCPF.Shared.Presentation` · `AgentConfig.cs`

## Description

AgentConfig is an abstract ScriptableObject. It serves two purposes: it creates the collision shape the map is baked with, and it tells drawing code the size and anchor of the body.

The package provides one implementation per shape, all in `NCPF.Config`:

| Config | Shape |
|---|---|
| [RectangularConfig](../Config/RectangularConfig.md) | A rectangle. |
| [CircleConfig](../Config/CircleConfig.md) | A circle. |
| [CapsuleConfig](../Config/CapsuleConfig.md) | A rectangle with rounded ends. |
| [TriangleConfig](../Config/TriangleConfig.md) | An isosceles triangle pointing forward. |
| [CompositeConfig](../Config/CompositeConfig.md) | Any combination of the shapes above, placed and rotated in the Inspector. |

To support another shape, write an [Agent](../Domain/Collision/Agent.md). If it is serializable, it can already be used as a part of a CompositeConfig; for a config of its own, derive from AgentConfig and return it from `GetAgent`.

## Key API

```csharp
Agent GetAgent(float cellSize)
```

Creates the collision shape at the given cell size. The map bake rotates it through every heading and stamps it over the obstacles.

| Parameter | Purpose |
|---|---|
| `cellSize` | The grid's cell size in metres, so the shape can be expressed in cells. |

```csharp
Vector2 FootprintSize { get; }
Vector2 FootprintAnchor { get; }
```

The body's bounding rectangle in metres and its anchor as a fraction of it.

## Inspector preview

Every AgentConfig, including your own, gets a preview below its fields: the body as the grid of cells it will be baked as, white where it covers a cell, with the anchor cell in blue. The anchor cell is the one placed on the agent's position.

| Field | Purpose |
|---|---|
| **Preview Cell Size** | The cell size to draw the body with. Set it to your grid's cell size to see exactly what the bake will use. Not stored in the asset. |
| **Preview Angle** | The heading to draw the body at, in degrees. Not stored in the asset. |

## Related

- [Agent](../Domain/Collision/Agent.md) — the collision shapes.
- [NCPFMapBuilder](../Bake/Presentation/NCPFMapBuilder.md) — bakes the map for the shape.
- [PathPlanner](../Pipeline/Presentation/PathPlanner.md) — draws the body along the plan.
