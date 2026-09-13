# MapBuilder

Bakes a [Map3D](Map3D.md) from a 2D obstacle grid and a body shape.

`NCPF.Domain` · `MapBuilder.cs`, `Map.cs`, `ConvolutionUtility.cs`

## Description

For each heading, the builder rotates the body's footprint and slides it over the whole grid. A cell is free at that heading only if the footprint, placed with its anchor on the cell, covers no obstacle. This is a convolution of the obstacle grid with the footprint.

Cost grows with *cells × headings × footprint cells*, so a large body on a fine grid takes noticeably longer to bake.

## Constructor

```csharp
MapBuilder(Map worldMap, Agent agent)
```

| Parameter | Purpose |
|---|---|
| `worldMap` | The obstacles: `Map(bool[,] obstacles, float cellSize)`, `true` = blocked. |
| `agent` | The body shape. Its heading is changed during the bake and reset to `0` afterwards. |

## Key API

```csharp
Map3D BuildMap(int angleStepCount)
```

Bakes a map with `angleStepCount` headings evenly spread over a full turn. Use a multiple of 4.

## Notes

- **Off-map cells count as free.** The part of a footprint that hangs past the grid's edge is not checked, so near the border the body may extend outside the grid. Surround the playable area with obstacles if that matters.
- The body's heading field is overwritten by the bake, which is why a heading set on the shape has no effect. See [RectangularConfig](../../Bake/Presentation/RectangularConfig.md).

## Related

- [Agent](Agent.md) — the body shape.
- [Map3D](Map3D.md) — the result.
- [NCPFMapBuilder](../../Bake/Presentation/NCPFMapBuilder.md) — runs the bake from the Editor.
