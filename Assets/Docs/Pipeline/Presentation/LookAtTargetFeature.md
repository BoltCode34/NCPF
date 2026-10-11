# LookAtTargetFeature

A main planner that keeps the agent's head on the target wherever it can. At every moment the agent either faces the target and moves slowly, or faces along the path and hurries — never somewhere in between — and every switch between the two takes as long as its turn rate demands.

`NCPF.Pipeline.Presentation` · `LookAtTargetFeature.cs`

## Description

Speed and facing cannot be planned separately: how far the body can turn over a stretch of path depends on how long it spends there. So this feature plans both together, over the next few metres of the path.

It samples the path ahead into a [MotionWindow](../../Pipeline/Application/Agent/MotionWindow.md) and searches a [FacingModeMap](../../Pipeline/Application/Space/FacingModeMap.md): at every sample the agent is in one of two **facing modes**, at one of a few speeds.

| Mode | The head points | Speed |
|---|---|---|
| **Target** | Straight at the target. | At most **Aim Speed Factor** × top speed. |
| **Path** | Along the path, the way the body was planned to be oriented. On a reversing curve that is against the direction of travel. | Up to top speed. |

A step of the plan is possible only if the speed change fits the acceleration, the speed fits the curvature, the facing change fits the turn rate over the time the step takes, and the body stays clear of obstacles while it turns and moves. That makes the target mode unavailable wherever the body would hit a wall facing the target, or the bearing to the target swings faster than the agent can turn.

The plan minimises the **time spent not facing the target**. Moving slowly while facing the target costs nothing, so the agent faces the target wherever it is feasible; where it is not, the agent switches to the path mode, speeds up to get the stretch over with, and slows down in time to turn back. Ending the planning window in the path mode also costs the time the turn back to the target would take, so a short window does not make the agent ride away from the target.

Between plans, the pass keeps the chosen mode, not an angle: in the target mode the head tracks the live bearing to the target, in the path mode the live tangent under the agent. It turns toward it no faster than the maximum turn rate.

## Inspector

### Planning Window

| Field | Purpose |
|---|---|
| **Lookahead** | How far ahead the plan looks, in metres. Longer windows switch modes more smartly but cost more to search. |
| **Sample Step** | Distance between samples along the path, in metres. |
| **Speed Count** | How many speed values the plan chooses from, zero included, evenly spaced up to top speed. |
| **Max Span** | The largest number of samples one step of the plan may skip. A turn has to fit into one step, so larger values allow larger turns at the same speed. |
| **Max Iterations** | The node budget for one plan search. |
| **Plan Interval** | How often the plan is recomputed, in seconds. |

### Facing

| Field | Purpose |
|---|---|
| **Aim Speed Factor** | The top speed while facing the target, as a fraction of the agent's top speed. It is raised to at least the lowest non-zero speed value, so the agent never has to stop to face the target. |

## Notes

- **Facing the target changes the body's orientation, so collisions depend on it.** The path was found for the body oriented along it; the target mode is only used where the map confirms the turned body fits.
- **A switch has to fit into one step of the plan.** If the agent cannot turn far enough within **Max Span** samples even at the lowest speed, it stays in the path mode. Raise **Max Span** or lower **Sample Step** for agents that turn slowly.
- **If the search fails,** the agent faces along the path at the aim speed until the next plan.

## Key API

```csharp
MainSAPass CreatePass(PathAgentContext context)
```

Creates a `LookAtTargetPass` for the agent in `context`, facing `context.Target`.

## Related

- [Planner features](PlannerFeatures.md) — the other features and how slots work.
- [TravelPlannerFeature](TravelPlannerFeature.md) — the standard main planner, always facing along the path.
- [FacingModeMap](../../Pipeline/Application/Space/FacingModeMap.md) — the graph it searches.
- [MotionWindow](../../Pipeline/Application/Agent/MotionWindow.md) — the sampled path.
- [LookPass](../../Pipeline/Application/Agent/LookPass.md) — a simpler override that turns toward the target without planning speed or checking collisions.
