# QuarterSymmetry

Rotates lattice cells by quarter turns. It lets the bake build curves for one quarter of the headings and rotate them into the other three.

`NCPF.Domain` · `QuarterSymmetry.cs`

## Description

A curve defined by its curvature looks the same whichever way it starts. Rotating the whole problem by 90° turns a curve from heading 0 into the matching curve from heading 90°, and on a square grid a 90° rotation maps cells onto cells exactly.

So the bake only searches for curves from the first quarter of the headings, and only that quarter is saved. When the set is read, [FuncControlSetContainer](../../Config/FuncControlSetContainer.md) uses QuarterSymmetry to rotate each saved curve's end cell into the other three quarters and rebuilds the curve from the rotated start heading with the same curvature.

With NCPF's angle convention, a quarter turn counter-clockwise maps cell `(x, y)` to `(−y, x)` and adds a quarter of the heading count to the heading index.

## Key API

```csharp
static int FullAngleCount(IGridGeometry3D space)
```

The number of headings in a full turn, from the grid's heading step. At least `4`.

```csharp
static Config RotateCell(Config cell, int quarter, int fullAngleCount)
```

Rotates `cell` by `quarter` quarter turns counter-clockwise, heading included. Negative values turn clockwise.

## Notes

- **The heading count must be a multiple of 4.** Otherwise a quarter turn is not a whole number of heading steps, and rotated curves start and end at the wrong headings.

## Related

- [ControlSetBuilder](../../Bake/Application/ControlSetBuilder.md) — bakes the first quarter.
- [FuncControlSetContainer](../../Config/FuncControlSetContainer.md) — rotates it into the rest on read.
