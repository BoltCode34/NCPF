# BezierPath

A cubic Bézier curve between two poses. It is not used by the package.

`NCPF.Domain` · `BezierPath.cs`

## Description

The curve starts and ends at the positions of the two poses; its shape is set by two free control points. Headings are taken from the curve's tangent.

BezierPath is kept as an example of a non-polynomial [IPath](../Core/IPath.md). It is not suitable for control sets:

- it cannot be saved, because it is not an `IPolynomialPath`;
- the headings at its ends follow the control points, not the poses passed in, so it does not necessarily match lattice headings;
- its curvature can jump, which the agent would feel as a snap in turning;
- arc length is approximated, and `Evaluate(s)` maps distance to the curve parameter linearly, so equal distances are not equally spaced on the curve.

## Constructor

```csharp
BezierPath(WorldConfig start, Vector2 p1, Vector2 p2, WorldConfig end)
```

| Parameter | Purpose |
|---|---|
| `start`, `end` | The end positions, and the poses reported as `Start` and `End`. |
| `p1`, `p2` | The two inner control points. |

## Related

- [IPath](../Core/IPath.md) — the curve contract.
- [ClothoidPath](ClothoidPath.md) — the curve the package uses.
