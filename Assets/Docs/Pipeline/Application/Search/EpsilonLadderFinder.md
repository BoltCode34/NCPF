# EpsilonLadderFinder

A background path finder that tries a sequence of heuristic weights, from greedy to careful, and returns the first path it finds.

`NCPF.Pipeline.Application` · `EpsilonLadderFinder.cs`, `EpsilonLadderTask.cs`, `EpsGoal.cs`

## Description

A* is guided by a heuristic that estimates the remaining cost. Multiplying that estimate by a weight above 1 makes the search greedier: it heads straight for the target and expands far fewer nodes, at the price of sometimes returning a longer path than the best one.

EpsilonLadderFinder uses this to find a usable path within a fixed budget. It runs the search once per weight in the ladder, in order, each with the same node budget, and returns the first path found. If a greedy rung runs out of budget — for example because a wall sends it the wrong way — the next rung tries again with a different weight.

The whole ladder runs on a thread-pool thread and is represented by an [IPathFindingTask](IAsyncGoalPathFinder.md) the caller can poll.

`EpsGoal` is the wrapper that applies a weight: it multiplies the wrapped goal's heuristic and leaves arrival untouched.

## Constructor

```csharp
EpsilonLadderFinder(IGoalPathFinder<TransitionData> finder, float[] epsilonLadder, int attemptBudget)
```

| Parameter | Purpose |
|---|---|
| `finder` | The search each rung runs. |
| `epsilonLadder` | The heuristic weights to try, in order. `1` is plain A*; higher values are greedier. An empty ladder uses a single rung of `2`. |
| `attemptBudget` | How many nodes each rung may expand. |

## Key API

```csharp
IPathFindingTask<TransitionData> FindPathAsync(IGraph<TransitionData> graph, int from, IGoal goal, int maxIterations = 1000)
```

Starts the ladder in the background and returns immediately. `maxIterations` is ignored; the budget from the constructor is used.

```csharp
TransitionData[] FindPath(IGraph<TransitionData> graph, int from, IGoal goal, int maxIterations = 1000)
```

Runs a single ordinary search synchronously, with no ladder and no weighting.

## Notes

- **Order the ladder deliberately.** Rungs run in the order given, and a rung is only reached if every rung before it failed. Put the weight you expect to succeed most often first. The Inspector default on PathPlanner, `{ 3, 2, 4, 1.5 }`, is not monotonic: a failed `3` is followed by a less greedy `2`, which is more expensive and likely to fail for the same reason.
- **Every rung starts from scratch.** No work carries over between rungs.

## Related

- [GoalPathFinderBase](GoalPathFinderBase.md) — the search each rung runs.
- [IAsyncGoalPathFinder](IAsyncGoalPathFinder.md) — the contract and the task it returns.
- [PathFindingAgent](PathFindingAgent.md) — uses it.
- [PathPlanner](../../../Pipeline/Presentation/PathPlanner.md) — where the ladder and budget are set.
