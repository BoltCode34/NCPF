# IGoalBuilder

Creates the goal for a search from its start and end nodes. Swapping the builder changes *what* is searched for without touching the search.

`NCPF.Pipeline.Application` · `IGoalBuilder.cs`, `GoalBuilderInterfaces.cs`, `BaseGoalBuilder.cs`

## Description

A goal can be cheap to create, like "reach this cell", or expensive, like a distance field over the whole map. So every builder contract comes in two forms:

| Asynchronous | Synchronous |
|---|---|
| `IAsyncGoalBuilder` → `Task<IGoal> Create(Config start, Config end)` | `IGoalBuilder` → `IGoal Create(Config start, Config end)` |
| `IAsyncAchivementBuilder` → `Task<IAchivementHandler> Create(...)` | `IAchivementBuilder` → `IAchivementHandler Create(...)` |
| `IAsyncHeuristicBuilder` → `Task<IHeuristicHandler> Create(...)` | `IHeuristicBuilder` → `IHeuristicHandler Create(...)` |

The search always calls the asynchronous form. Each synchronous interface derives from its asynchronous one and implements the async `Create` by calling the sync one. So:

- **A cheap builder** implements only the synchronous `Create`. It works everywhere.
- **An expensive builder** also overrides the asynchronous `Create` to do the work off the main thread.

## BaseGoalBuilder

The standard goal builder. It holds an achievement builder and a heuristic builder and pairs their results into a [GoalBase](IGoal.md).

```csharp
IAchivementBuilder AchivementBuilder;
IHeuristicBuilder HeuristicBuilder;

IGoal Create(Config start, Config end)
```

[PathPlanner](../../../Pipeline/Presentation/PathPlanner.md) fills both fields with the live slots from [BehaviorBus](../Agent/BehaviorBus.md), so the achievement and heuristic chosen in the Inspector can change while the agent runs.

## Parameters of Create

| Parameter | Purpose |
|---|---|
| `start` | The node the search starts from. |
| `end` | The node of the target. Achievements and heuristics measure against it. |

## Related

- [IGoal](IGoal.md) — what is created.
- [Planner features](../../../Pipeline/Presentation/PlannerFeatures.md) — features that create builders.
- [PathFindingAgent](PathFindingAgent.md) — builds the goal before each search.
