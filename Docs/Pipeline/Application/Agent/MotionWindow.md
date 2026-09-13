# MotionWindow

The next few metres of an agent's path, sampled once per plan, together with the rules that price facing and speed along them. Used by [MotionPlannerFeature](../../../Pipeline/Presentation/MotionPlannerFeature.md).

`NCPF.Pipeline.Application` · `MotionWindow.cs`

## Description

The motion planner searches many possible facing and speed values at every sample along the path, and every one of them needs the same geometry: where the sample is, how far along the path, how curved, and which way the target is. MotionWindow computes that once per plan and stores it in arrays, instead of resampling the path for each of thousands of queries.

The file also contains the pieces the motion planner's search is built from.

| Type | Role |
|---|---|
| `MotionWindow` | The sampled path: position, distance along, curvature, travel direction and direction to the target at each sample. |
| `MotionRules` | Prices a facing and speed at a sample. See below. |
| `MotionRulesGraph` | Adds the rules' price to the weight of every edge of the facing-and-speed graph. |
| `WindowEndAchivement` | The search is done when it reaches the end of the window, at any facing and speed. |
| `WindowTimeHeuristic` | Estimates the remaining time as the remaining distance at top speed. |

### The rules

Every facing and speed is priced by two modes, and pays the cheaper:

- **Aim** — cost grows with how far the facing is from the target direction and how far the speed is from the slow speed.
- **Travel** — a fixed bias, plus cost that grows with how far the facing is from the travel direction and how far the speed is from the fast speed.

Paying only the cheaper mode puts a ridge between the two, so the plan commits to one mode rather than hovering between them.

The rules only set prices. Adding the price to the search goes through a graph decorator, `MotionRulesGraph`, which leaves the underlying graph unaware of any preference. Swapping the decorator makes the same graph plan a different behaviour.

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

- [MotionPlannerFeature](../../../Pipeline/Presentation/MotionPlannerFeature.md) — the main planner that uses it.
- [SpacedMapAV](../Space/SpacedMapAV.md) — the facing-and-speed graph over the window.
