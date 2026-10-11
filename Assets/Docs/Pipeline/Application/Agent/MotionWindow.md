# MotionWindow

The next few metres of an agent's path, sampled once per plan, with the geometry every sample needs. Used by [LookAtTargetFeature](../../../Pipeline/Presentation/LookAtTargetFeature.md).

`NCPF.Pipeline.Application` · `MotionWindow.cs`

## Description

The facing planner asks about every sample along the path many times, and every question needs the same geometry: where the sample is, how far along the path, how curved, which way the path heads and which way the target is. MotionWindow computes that once per plan and stores it in arrays, instead of resampling the path for each of thousands of queries.

| Field | Meaning |
|---|---|
| `Count` | The number of samples. |
| `Pos` | The position of each sample. |
| `Arc` | The distance along the path from the first sample. |
| `Curvature` | The curvature at each sample, in radians per metre. |
| `Cx`, `Cy` | The grid cell of each sample. |
| `Travel` | The path's heading at each sample, in degrees. |
| `Aim` | The direction from each sample to the target, in degrees. |

The file also contains the goal of the facing planner's search:

| Type | Role |
|---|---|
| `WindowEndAchivement` | The search is done when it reaches the last sample, in any mode and at any speed. |
| `ZeroHeuristic` | No estimate of the remaining cost. |

## Constructor

```csharp
MotionWindow(IReadOnlyList<WorldConfig> poses, IGridMap3D grid, Vector2 target)
```

| Parameter | Purpose |
|---|---|
| `poses` | The sampled poses ahead of the agent. |
| `grid` | Converts the samples to cells. |
| `target` | Where the target is, to compute the aim direction. |

## Related

- [LookAtTargetFeature](../../../Pipeline/Presentation/LookAtTargetFeature.md) — the main planner that uses it.
- [FacingModeMap](../Space/FacingModeMap.md) — the graph over the window.
