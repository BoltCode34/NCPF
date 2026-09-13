# IPath

A curve: a pose for every distance travelled along it. Every curve in NCPF, from the control set to the path an agent drives, is an IPath.

`NCPF.Domain` · `IPath.cs`

## Description

```csharp
public interface IPath
{
    float Length { get; }
    WorldConfig Start { get; }
    WorldConfig End { get; }
    WorldConfig Evaluate(float s);
    IReadOnlyList<WorldConfig> Sample(float step);
}
```

A path is parameterised by **arc length**, in metres: `Evaluate(s)` returns the position and heading after travelling `s` metres from the start. There is no time and no speed in a path. Speed is planned separately, while driving.

The heading a path reports is the direction the **body's front** points, not necessarily the direction of travel. On a curve driven in reverse ([RearFirstPath](../Paths/RearFirstPath.md)) the body moves opposite to its heading.

## Members

| Member | Meaning |
|---|---|
| `Length` | The total arc length, in metres. |
| `Start`, `End` | The poses at `0` and at `Length`. |
| `Evaluate(s)` | The pose at `s` metres. Values outside `0..Length` are clamped. |
| `Sample(step)` | Poses every `step` metres, including the end. |

## IPolynomialPath

```csharp
public interface IPolynomialPath : IPath
{
    float A { get; } float B { get; } float C { get; } float D { get; }
}
```

A path whose curvature along its length is the cubic `κ(s) = A·s³ + B·s² + C·s + D`, in radians per metre. A clothoid is the special case `A = B = 0`.

Control sets are saved as these coefficients plus the length, so only polynomial paths can be stored in a [FuncControlSetContainer](../../Config/FuncControlSetContainer.md). The coefficients also let [PrimitiveAnalyzer](../Agent/PrimitiveAnalyzer.md) compute peak curvature exactly.

## IPathWrapper

```csharp
public interface IPathWrapper { IPath Inner { get; } }
```

Implemented by paths that wrap another path, such as [PathOffset](../../Pipeline/Application/Space/PathOffset.md). Code that needs the original, such as reverse detection in [PathGearUtility](../Agent/PathGearUtility.md), looks through wrappers with it.

## Implementing a path

Implement IPath for a new kind of curve. Keep `Evaluate` cheap: it is called many times per frame while driving. If the curvature is a cubic polynomial, implement `IPolynomialPath` so the path can be saved. If the path wraps another, implement `IPathWrapper`.

## Related

- [WorldConfig](WorldConfig.md) — the pose type.
- [ClothoidPath](../Paths/ClothoidPath.md), [FunctionBasedPath](../Paths/FunctionBasedPath.md), [RearFirstPath](../Paths/RearFirstPath.md), [BezierPath](../Paths/BezierPath.md) — implementations.
