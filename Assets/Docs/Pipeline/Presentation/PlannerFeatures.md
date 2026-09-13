# Planner features

Features are how you choose an agent's behaviour in the Inspector. Each feature is a small serializable class that creates a piece of runtime logic — a pass or a goal builder — for the planner.

`NCPF.Pipeline.Presentation` · `MainPlannerFeature.cs`, `PlannerOverrideFeature.cs`, `AchivementFeature.cs`, `HeuristicFeature.cs`

## Description

[PathPlanner](PathPlanner.md) has four feature slots. Each slot shows a dropdown listing every class that derives from the slot's base type, including your own. An empty slot uses the standard feature from [PlannerStandards](PlannerStandards.md).

| Slot | Base type | Decides | Standard |
|---|---|---|---|
| **Main Planner Feature** | `MainPlannerFeature` | How the agent chooses its speed and facing along the path. | [TravelPlannerFeature](TravelPlannerFeature.md) |
| **Achivement Feature** | `AchivementFeature` | When the target counts as reached. | `PointCellAchivementFeature` |
| **Heuristic Feature** | `HeuristicFeature` | How the search estimates the remaining cost. | `DirectDistHeuristicFeature` |
| **Overrides** | `PlannerOverrideFeature` | Optional behaviours on top of the main planner. | none |

You can change a feature while the game runs. The running search and follower pick up the new logic without restarting, through [BehaviorBus](../../Pipeline/Application/Agent/BehaviorBus.md).

## Built-in features

### Main planners

| Feature | Behaviour |
|---|---|
| [TravelPlannerFeature](TravelPlannerFeature.md) | Drives the path at the fastest speed its curves allow, facing along the path. |
| [MotionPlannerFeature](MotionPlannerFeature.md) | Plans speed and facing together, switching between driving fast along the path and slowing down to face the target. |

### Achievements

| Feature | Behaviour |
|---|---|
| `PointCellAchivementFeature` | Reached when the agent is in the target's cell, at any heading. No settings. See [PointCellAchivementBuilder](../../Pipeline/Application/Search/PointCellAchivementBuilder.md). |
| `RingAchivementFeature` | Reached anywhere in a ring around the target, between **Min Distance** and **Max Distance** metres. Use it to approach and keep a distance. Keep the ring at least a couple of cells wide, or it may contain no reachable node. See [RingAchivementBuilder](../../Pipeline/Application/Search/RingAchivementBuilder.md). |

### Heuristics

| Feature | Behaviour |
|---|---|
| `DirectDistHeuristicFeature` | Straight-line distance divided by top speed. Free to compute, but ignores walls. See [DirectDistHeuristicsBuilder](../../Pipeline/Application/Search/DirectDistHeuristicsBuilder.md). |
| `DijkstraHeuristicFeature` | A precomputed distance field that accounts for walls and turning. Much more accurate in cluttered levels, but sweeps the whole map whenever the target moves to a new cell. **Turn Penalty Cells** sets how many cells a turn is worth. See [DijkstraHeuristicBuilder](../../Pipeline/Application/Search/DijkstraHeuristicBuilder.md). |

## Writing your own

Derive from one of the four base types, mark the class `[Serializable]`, give it a parameterless constructor, and implement the one abstract method. It appears in the slot's dropdown automatically.

```csharp
[Serializable]
public class LookAtTargetFeature : PlannerOverrideFeature
{
    public override SAPass CreatePass(PathAgentContext context)
        => new LookPass(new LookAt(context.Target), context.DynamicAgent.MaxAngledVelocity);
}
```

| Base type | Implement |
|---|---|
| `MainPlannerFeature` | `MainSAPass CreatePass(PathAgentContext context)` |
| `PlannerOverrideFeature` | `SAPass CreatePass(PathAgentContext context)` |
| `AchivementFeature` | `IAchivementBuilder CreateBuilder(PathAgentContext context)` |
| `HeuristicFeature` | `IHeuristicBuilder CreateBuilder(PathAgentContext context)` |

The feature only creates the logic. Keep the logic itself in the pass or builder, which lives in the application layer and knows nothing about the Inspector.

## Related

- [SAPass](../../Pipeline/Application/Agent/SAPass.md) — what main planners and overrides create.
- [IGoalBuilder](../../Pipeline/Application/Search/IGoalBuilder.md) — what achievements and heuristics create.
- [PathFeatureContainer](PathFeatureContainer.md) — turns the selected features into live logic.
- [Pipeline editors](../Editor/PipelineEditors.md) — the dropdown that lists the features.
