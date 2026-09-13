# PathOffset

A curve moved to another position without rotating it.

`NCPF.Pipeline.Application` · `PathOffset.cs`

## Description

The control set stores each curve as it starts from the origin. To use it at a lattice node, it has to be moved there. PathOffset wraps the stored curve and adds a world offset to every point it returns. Nothing is copied.

It also exposes the stored curve through `Inner` ([IPathWrapper](../../../Domain/Core/IPath.md)), so code that needs the original, such as curvature analysis, can reach it. Moving a curve does not change its curvature.

## Constructor

```csharp
PathOffset(IPath inner, Vector2 shift)
```

| Parameter | Purpose |
|---|---|
| `inner` | The curve to move. |
| `shift` | The world offset to add. |

## Related

- [IPath](../../../Domain/Core/IPath.md) — the curve contract.
- [PlanGeometry](../Search/PlanGeometry.md) — places curves this way.
