# Agent

A body shape for map baking: which grid cells the body covers at a given heading, and which of those cells it is placed by. The base class of every shape: [Rectangle](#rectangle), [Circle](#circle), [Capsule](#capsule), [Triangle](#triangle) and [CompositeAgent](CompositeAgent.md).

`NCPF.Domain` · `Agent.cs`, `Rectangle.cs`, `Circle.cs`, `Capsule.cs`, `Triangle.cs`

## Description

To bake a map, [MapBuilder](MapBuilder.md) needs the body as a grid of cells, its **footprint**, at every heading. Agent builds that grid; a concrete shape only answers one question — does it overlap a given cell.

Every shape is inscribed in a **bounding rectangle**, `Size`: `x` across the heading, `y` along it. At heading 0 the heading points along +Y.

- **The footprint** is a square, odd-sized grid, sized by the bounding rectangle's diagonal so it fits every rotation. A cell counts as covered if the shape overlaps it at all, so the footprint is slightly larger than the body.
- **The anchor** is the footprint cell that sits on the lattice node. When the agent drives, the node is the point that follows the path exactly, so the anchor must match the point the planner moves: for a body moved by its centre, the centre; for a needle pulled by its tip, the tip. It is given as a fraction of the bounding rectangle — `(0, 0)` the lower-left corner, `(1, 1)` the upper-right, `(0.5, 1)` the middle of the front edge — and rotates with the body.

Shapes are `[Serializable]` and serialize only their own dimensions, so a config can store them by reference and edit them in the Inspector. Position, heading, cell size and anchor are bake-time state and are not serialized.

## Key API

```csharp
bool[,] Footprint { get; }
Int2 Anchor { get; }
Vector2 Size { get; }
```

The covered cells, the anchor cell, and the bounding rectangle in metres, for the current `Angle` and `CellSize`.

| Field | Meaning |
|---|---|
| `Angle` | The heading to build the footprint for, in degrees. Set by the bake. |
| `CellSize` | The size of a footprint cell. Must equal the grid's. |
| `AnchorFraction` | The anchor as a fraction of the bounding rectangle. Defaults to the centre. |
| `X`, `Y` | Not used by the bake. |

## Implementing a shape

```csharp
public abstract Vector2 Size { get; }
protected internal abstract bool Covers(Vector2 cellCenter, Vector2 axisX, Vector2 axisY, float halfCell);
```

- `Size` returns the bounding rectangle in metres.
- `Covers` answers whether the shape, centred at the origin and rotated so that its local axes are `axisX` and `axisY`, overlaps the axis-aligned cell centred at `cellCenter` with half size `halfCell`. It must not depend on `Angle` or `CellSize` — everything it needs is passed in, which is what lets [CompositeAgent](CompositeAgent.md) place a shape anywhere inside itself.

In an assembly other than `NCPF.Domain`, override `Covers` as `protected override`. Mark the class `[Serializable]` and give it a parameterless constructor to make it selectable in a [CompositeConfig](../../Config/CompositeConfig.md). `GetFootPrint` and `GetLocalAnchor` are virtual for shapes that need a different grid altogether.

## Rectangle

```csharp
Rectangle(float width, float height, float cellSize = 1f, Vector2? anchor = null)
```

A rectangle of `Width × Height` metres, `Height` along the heading. The overlap test is exact.

## Circle

```csharp
Circle(float radius, float cellSize = 1f, Vector2? anchor = null)
```

A circle of `Radius` metres. Its bounding rectangle is a square of `2 · Radius`, so an anchor of `(0.5, 1)` is the front point of the circle, and `(0, 0)` lies outside the body.

## Capsule

```csharp
Capsule(float width, float length, float cellSize = 1f, Vector2? anchor = null)
```

A `Width`-diameter disc swept along the heading, `Length` metres long including the rounded ends. A `Length` shorter than `Width` gives a circle.

## Triangle

```csharp
Triangle(float width, float height, float cellSize = 1f, Vector2? anchor = null)
```

An isosceles triangle: a `Width`-wide base at the back and the apex at the middle of the front edge, `Height` along the heading. An anchor of `(0.5, 1)` is the apex.

## Related

- [CompositeAgent](CompositeAgent.md) — a shape assembled from other shapes.
- [AgentConfig](../../Shared/AgentConfig.md) — creates a shape for the bake.
- [MapBuilder](MapBuilder.md) — uses the footprint.
