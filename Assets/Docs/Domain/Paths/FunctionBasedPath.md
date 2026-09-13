# FunctionBasedPath

A curve defined by how its curvature changes along its length. The path is built by integrating that curvature from the start pose.

`NCPF.Domain` · `FunctionBasedPath.cs`, `ICurvatureFunction.cs`, `CubicPolyniomFunction.cs`

## Description

Given a curvature function `κ(s)` and a start pose, FunctionBasedPath steps along the length: the heading turns by `κ` per metre, and the position moves along the heading. It stores a pose every `minStep` metres, and `Evaluate` refines from the nearest stored pose with smaller steps.

`PolynomialBasedPath` is the version for a cubic curvature, `κ(s) = A·s³ + B·s² + C·s + D`. It implements [IPolynomialPath](../Core/IPath.md), so it can be saved. [FuncControlSetContainer](../../Config/FuncControlSetContainer.md) uses it to rebuild saved curves that are not plain clothoids.

## Curvature functions

```csharp
public interface ICurvatureFunction
{
    float L { get; }
    float Evaluate(float s);
}
```

`L` is the length of the curve, and `Evaluate` returns curvature in radians per metre. Implement it to define a new kind of curve. `CubicPolyniomFunction` is the cubic, with an optional limit on absolute curvature.

## Constructor

```csharp
FunctionBasedPath(T function, WorldConfig start, WorldConfig end, float minStep = 0.1f)
PolynomialBasedPath(ICubicPolynomialFunction function, WorldConfig start, WorldConfig end, float minStep = 0.1f)
```

| Parameter | Purpose |
|---|---|
| `function` | The curvature along the length. Its `L` is the path's length. |
| `start` | The pose integration starts from. |
| `end` | The pose reported at the end of the path. |
| `minStep` | The distance between stored poses, in metres. Smaller is more accurate and uses more memory. |

## Notes

- **The end is pinned, not computed.** At `Length`, the path returns `end` exactly, while just before it returns the integrated pose. If the function does not actually lead to `end`, the curve jumps at its last point.
- **`CubicPolyniomFunction` has a minimum length of 0.1 m.** A shorter `L` is raised to 0.1.
- `Evaluate` searches the stored poses from the start each time, so it becomes slower on long curves with a small `minStep`.

## Related

- [ClothoidPath](ClothoidPath.md) — the analytic clothoid.
- [IPath](../Core/IPath.md) — the curve contract.
