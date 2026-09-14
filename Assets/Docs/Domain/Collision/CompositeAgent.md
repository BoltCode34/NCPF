# CompositeAgent

A body shape assembled from other shapes, each placed and rotated inside it. Use it for bodies no single primitive describes, such as an I-beam.

`NCPF.Domain` · `CompositeAgent.cs`

## Description

A composite holds a list of **parts**. Each part is a shape ([Agent](Agent.md)), the offset of that shape's centre inside the composite, and its rotation.

- **Covered cells.** A cell is covered when any part covers it.
- **Bounding rectangle.** The union of the parts' rotated bounding rectangles. The composite's centre is the centre of that union, not the origin of the offsets, so offsets only matter relative to each other.
- **Anchor.** A fraction of the union rectangle, like every other shape.

Parts with no shape are ignored. A composite with no parts has a zero size and covers nothing.

The composite itself is not serializable and is not offered in the shape dropdown, so composites cannot be nested. It is created on every request by [CompositeConfig](../../Config/CompositeConfig.md), which stores the parts.

## Constructor

```csharp
CompositeAgent(IReadOnlyList<Part> parts, float cellSize = 1f, Vector2? anchor = null)
```

| Parameter | Purpose |
|---|---|
| `parts` | The shapes and their placement. |
| `cellSize` | The grid's cell size. |
| `anchor` | The anchor as a fraction of the union rectangle. Defaults to the centre. |

## Part

```csharp
Part(Agent shape, Vector2 offset, float rotation)
```

| Field | Meaning |
|---|---|
| `Shape` | The shape. Its own position, heading and anchor are ignored. |
| `Offset` | The centre of the shape's bounding rectangle inside the composite, in metres: `x` across the heading, `y` along it. |
| `Rotation` | The shape's rotation inside the composite, in degrees, counter-clockwise. |

## Notes

- **The parts share the object instances from the config.** The composite only reads them, so this is safe for baking, but do not change a part's fields from code while a bake is running.

## Related

- [Agent](Agent.md) — the shapes a composite is made of.
- [CompositeConfig](../../Config/CompositeConfig.md) — sets up a composite in the Inspector.
