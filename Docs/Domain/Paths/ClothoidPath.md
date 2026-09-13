# ClothoidPath

A clothoid (Euler spiral): a curve whose curvature changes linearly with distance. It is the standard curve NCPF bakes into control sets.

`NCPF.Domain` · `ClothoidPath.cs`

## Description

On a clothoid, the steering changes at a constant rate, the way a driver turns a wheel smoothly. There are no sudden jumps in curvature, so an agent following one never has to snap its turning rate.

A ClothoidPath connects two poses exactly, matching both positions and both headings. The solving is done by the bundled ClothoidX library; ClothoidPath wraps the result and converts its angles to the NCPF convention.

As an [IPolynomialPath](../Core/IPath.md), a clothoid has `A = B = 0`, `C` = sharpness (how fast curvature changes per metre) and `D` = starting curvature.

## Creating one

Clothoids are normally created by the bake:

- `ClothoidPathBuilder` solves one between two poses;
- [FuncControlSetContainer](../../Config/FuncControlSetContainer.md) rebuilds them from saved coefficients.

```csharp
ClothoidPath(ClothoidSegment segment, WorldConfig start, WorldConfig end)
```

| Parameter | Purpose |
|---|---|
| `segment` | The solved ClothoidX segment. |
| `start`, `end` | The poses to report as `Start` and `End`. |

## Related

- [IPathBuilder](../../Bake/Application/IPathBuilder.md) — how the bake creates curves.
- [RearFirstPath](RearFirstPath.md) — driving a clothoid in reverse.
- [FunctionBasedPath](FunctionBasedPath.md) — the general cubic.
