# AgentConfig

The port for an agent's body shape. The map is baked for the shape an AgentConfig describes.

`NCPF.Shared.Presentation` · `AgentConfig.cs`

## Description

AgentConfig is an abstract ScriptableObject. The standard implementation is [RectangularConfig](../Bake/Presentation/RectangularConfig.md).

It serves two purposes: it creates the collision shape the map is baked with, and it tells drawing code the size and anchor of the body.

To support another shape, derive from AgentConfig and return your own [Agent](../Domain/Collision/Agent.md) from `GetAgent`.

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

The body's size in metres and its anchor as a fraction of that size. Used to draw the body.

## Related

- [RectangularConfig](../Bake/Presentation/RectangularConfig.md) — the standard implementation.
- [Agent](../Domain/Collision/Agent.md) — the collision shape.
- [NCPFMapBuilder](../Bake/Presentation/NCPFMapBuilder.md) — bakes the map for the shape.
