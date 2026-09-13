# MotionPlannerFeature

A main planner that decides speed and facing together. The agent switches between two modes: driving fast while facing along the path, and slowing down while facing the target.

`NCPF.Pipeline.Presentation` · `MotionPlannerFeature.cs`

## Description

Speed and facing cannot be planned separately: how far the agent can turn over a stretch of path depends on how long it spends there. Slowing down buys time to turn.

This feature samples the next few metres of the path into a [MotionWindow](../../Pipeline/Application/Agent/MotionWindow.md) and searches a graph of facing and speed values along it ([SpacedMapAV](../../Pipeline/Application/Space/SpacedMapAV.md)). Every state is priced by two rules, and the agent pays the cheaper of the two:

- **Aim** — face the target and move at the slow speed.
- **Travel** — face along the path and move at the fast speed.

Because each state pays only the cheaper mode, hovering half-way between them is never the best choice, so the agent commits to one mode instead of averaging.

## Inspector

### Planning Window

| Field | Purpose |
|---|---|
| **Lookahead** | How far ahead the plan looks, in metres. |
| **Sample Step** | Distance between samples along the path, in metres. |
| **Speed Count** | How many speed values the plan chooses from. |
| **Max Span** | The largest number of samples one step of the plan may skip. |
| **Max Iterations** | The node budget for one plan search. |
| **Plan Interval** | How often the plan is recomputed, in seconds. |

### Rules

| Field | Purpose |
|---|---|
| **Slow Speed Factor** | The aim mode's speed, as a fraction of top speed. |
| **Fast Speed Factor** | The travel mode's speed, as a fraction of top speed. |
| **Angle Weight** | Cost per degree the facing is away from the mode's direction. |
| **Speed Weight** | Cost per metre per second the speed is away from the mode's speed. |
| **Travel Bias** | A constant added to the travel mode's cost. Higher values make the agent turn to face the target more readily. |

## Key API

```csharp
MainSAPass CreatePass(PathAgentContext context)
```

Creates a `MotionPlannerPass` for the agent in `context`, targeting `context.Target`.

## Related

- [Planner features](PlannerFeatures.md) — the other features and how slots work.
- [TravelPlannerFeature](TravelPlannerFeature.md) — the standard main planner.
- [MotionWindow](../../Pipeline/Application/Agent/MotionWindow.md) — the sampled path and the rules.
- [SpacedMapAV](../../Pipeline/Application/Space/SpacedMapAV.md) — the facing-and-speed graph it searches.
