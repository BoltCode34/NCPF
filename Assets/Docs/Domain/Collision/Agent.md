# Agent

A body shape for map baking: which grid cells the body covers at a given heading, and which of those cells it is placed by.

`NCPF.Domain` · `Agent.cs`, `Rectangle.cs`

## Description

To bake a map, [MapBuilder](MapBuilder.md) needs the body as a grid of cells, its **footprint**, at every heading. Agent is the base class for shapes that provide one.

The **anchor** is the footprint cell that sits on the lattice node. When the agent drives, the node is the point that follows the path exactly, so the anchor must match the point the planner moves: for a body moved by its centre, the centre; for a needle pulled by its tip, the tip.

## Implementing a shape

```csharp
public abstract bool[,] GetFootPrint();
public virtual Int2 GetLocalAnchor();
```

- `GetFootPrint` returns the covered cells for the current `Angle` and `CellSize`. Keep the array the same size at every heading, so the anchor stays consistent.
- `GetLocalAnchor` returns the anchor cell. By default, the centre of the covered cells.

| Field | Meaning |
|---|---|
| `Angle` | The heading to build the footprint for, in degrees. Set by the bake. |
| `CellSize` | The size of a footprint cell. Must equal the grid's. |
| `X`, `Y` | Not used by the bake. |

## Rectangle

The standard shape: a rectangle of `Width × Height` metres, with its height along the heading.

```csharp
Rectangle(float width, float height, float cellSize = 1f, Vector2? anchor = null)
```

| Parameter | Purpose |
|---|---|
| `width`, `height` | The size in metres. At heading 0, `height` runs along +Y. |
| `cellSize` | The grid's cell size. |
| `anchor` | The anchor as a fraction of the rectangle: `(0, 0)` is the lower-left corner, `(1, 1)` the upper-right, `(0.5, 1)` the middle of the front edge. Defaults to the centre. |

A cell counts as covered if the rectangle overlaps it at all, so the footprint is slightly larger than the body. The footprint array is square, sized by the rectangle's diagonal, so it fits every rotation.

## Related

- [RectangularConfig](../../Bake/Presentation/RectangularConfig.md) — sets up a Rectangle in the Inspector.
- [MapBuilder](MapBuilder.md) — uses the footprint.
