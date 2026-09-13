# TravelPlannerFeature

The standard main planner: the agent drives along its path as fast as the path's curves allow, facing along the path.

`NCPF.Pipeline.Presentation` · `TravelPlannerFeature.cs`

## Description

The planned path is pure geometry — it has no speed. This feature decides the speed. A few times per second it looks at the next few metres of the path and searches for a speed plan across the upcoming curves, using a small graph of speed values ([SpacedMapPV](../../Pipeline/Application/Space/SpacedMapPV.md)). A speed change is allowed only if the agent can physically make it:

- it can accelerate or brake between the two speeds over the length of the curve;
- it can take the curve at that speed without exceeding its acceleration;
- it can turn its body fast enough to follow the curve at that speed.

Along each curve, the speed then follows a [CurvatureSpeedProfile](../../Domain/Agent/CurvatureSpeedProfile.md), so the agent slows down exactly where the curve tightens and brakes in time for the next one.

Speed is signed: positive when driving forward, negative when reversing. Switching between them always passes through a full stop.

## Inspector

| Field | Purpose |
|---|---|
| **Target Speed Factor** | The highest speed the agent aims for, as a fraction of its top speed. |
| **Speed Strive** | How strongly the agent prefers to stay near that speed instead of settling for a slower safe plan. `0` is neutral. |
| **Lookahead** | How far ahead the speed plan looks, in metres. The window always extends to the end of a curve, never cutting one in half. |
| **Speed Steps** | How many speed values the plan chooses from in each direction. With 5, it picks from 5 forward speeds, 5 reverse speeds and a stop. More steps give smoother speed, at a higher cost. |
| **Plan Interval** | How often the speed plan is recomputed, in seconds. |
| **Max Iterations** | The node budget for one speed plan search. |

## Key API

```csharp
MainSAPass CreatePass(PathAgentContext context)
```

Creates a `TravelPlannerPass` for the agent in `context`, using its dynamics limits.

## Related

- [Planner features](PlannerFeatures.md) — the other features and how slots work.
- [MotionPlannerFeature](MotionPlannerFeature.md) — the alternative main planner that also plans facing.
- [SpacedMapPV](../../Pipeline/Application/Space/SpacedMapPV.md) — the speed graph it searches.
- [CurvatureSpeedProfile](../../Domain/Agent/CurvatureSpeedProfile.md) — the speed limit along one curve.
- [PrimitiveWindow](../../Domain/Agent/PrimitiveWindow.md) — the stretch of path it plans over.
