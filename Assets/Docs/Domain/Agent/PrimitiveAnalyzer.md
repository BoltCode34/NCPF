# PrimitiveAnalyzer

Measures a curve's sharpest point, its peak curvature.

`NCPF.Domain` · `PrimitiveAnalyzer.cs`, `PrimitiveAnalysis.cs`

## Description

Peak curvature decides how fast a curve can be driven, so the search uses it to price every edge. It depends only on the shape, so it is measured once per curve of the control set.

- **Polynomial curves** ([IPolynomialPath](../Core/IPath.md), including clothoids) are measured **exactly**: the coefficients describe the curvature directly, and its peak is found at the ends or where its derivative is zero.
- **Other curves** are measured by sampling the heading every 5 cm. A very narrow spike of curvature between samples can be missed.

## Key API

```csharp
static PrimitiveAnalysis Analyze(IPath path)
```

Returns a `PrimitiveAnalysis`:

| Field | Meaning |
|---|---|
| `KappaPeak` | The largest absolute curvature along the curve, in radians per metre. The tightest turning radius is `1 / KappaPeak`. |

## Notes

- Other parts of the package measure curvature their own way, by sampling: the bake's turning-radius check in [PathLimiterBase](PathLimiterBase.md), and [PrimitiveWindow](PrimitiveWindow.md) and [CurvatureSpeedProfile](CurvatureSpeedProfile.md) while driving. The values are close but not identical.

## Related

- [PassabilityTransitionGenerator](../../Pipeline/Application/Space/PassabilityTransitionGenerator.md) — prices edges with it.
