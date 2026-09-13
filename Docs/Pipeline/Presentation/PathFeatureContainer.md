# PathFeatureContainer

Turns the features selected on a [PathPlanner](PathPlanner.md) into live logic, and keeps the two in sync while the game runs.

`NCPF.Pipeline.Presentation` · `PathFeatureContainer.cs`

## Description

The planner calls `Bind` every frame. The container compares each feature slot with what it saw last time and, only when something changed:

- creates the new pass or goal builder from the feature and puts it into the live slot on the [BehaviorBus](../../Pipeline/Application/Agent/BehaviorBus.md);
- replaces an empty slot with the matching standard from [PlannerStandards](PlannerStandards.md);
- adds or removes override passes to match the **Overrides** list.

When nothing changed, `Bind` does nothing. A new container is created every time the planner is wired, so it starts without stale state.

## Constructor

```csharp
PathFeatureContainer(PlannerStandards standards)
```

| Parameter | Purpose |
|---|---|
| `standards` | The features to use for empty slots. |

## Key API

```csharp
void Bind(BehaviorBus bus, PathAgentContext context,
          MainPlannerFeature main, AchivementFeature achivement,
          HeuristicFeature heuristic, List<PlannerOverrideFeature> overrides)
```

Brings the live logic on `bus` in line with the given features.

| Parameter | Purpose |
|---|---|
| `bus` | The live behaviour slots to update. |
| `context` | Passed to the features when they create their logic. |
| `main`, `achivement`, `heuristic` | The selected features. `null` means use the standard. |
| `overrides` | The optional behaviours that should be active. |

## Related

- [Planner features](PlannerFeatures.md) — the features it handles.
- [BehaviorBus](../../Pipeline/Application/Agent/BehaviorBus.md) — where the live logic is placed.
- [PlannerStandards](PlannerStandards.md) — the fallbacks for empty slots.
