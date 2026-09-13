# GoalPathFinderBase

The A\* search NCPF runs everywhere: over the lattice of curves, and over the small speed and facing graphs of the main planners.

`NCPF.Pipeline.Application` · `GoalPathFinderBase.cs`, `CurvateGoalPathFinder.cs`

## Description

GoalPathFinderBase is generic in the edge type, so the same code searches any graph. `CurvateGoalPathFinder` is the version for the lattice of curves, `GoalPathFinderBase<TransitionData>`.

Instead of a destination node, the search takes a **goal** ([IGoal](IGoal.md)): a test for "have I arrived" and an estimate of the remaining cost. This lets the same search reach a single cell, any cell in a ring, or the end of a planning window.

The search keeps all its working data local to each call, so one instance can run several searches at once on different threads.

It expands nodes in order of cost so far plus estimated cost remaining, skips nodes the graph reports as blocked, and stops as soon as it removes a node that satisfies the goal from the queue.

## Key API

```csharp
T[] FindPath(IGraph<T> graph, int from, IGoal goal, int maxIterations)
```

Searches from `from` until the goal is achieved or the budget runs out.

| Parameter | Purpose |
|---|---|
| `graph` | The graph to search. |
| `from` | The start node. |
| `goal` | Decides when to stop and estimates the remaining cost. |
| `maxIterations` | How many nodes may be expanded before giving up. |

Returns the edges of the path from `from` to the arrival node. Returns an empty array if the start node is blocked, the budget runs out, or no path exists.

## Notes

- **The estimate must not overestimate** for the path to be the cheapest. An estimate that overestimates, such as a weighted one from [EpsilonLadderFinder](EpsilonLadderFinder.md), finds paths faster but not always the cheapest.
- Once a node has been expanded it is never expanded again, even if a cheaper way to it turns up later.
- The goal's per-node cost ([ICostHandler](IGoal.md)) is **not** read. To make some nodes cost more, decorate the graph's edge weights instead.

## Related

- [IGoal](IGoal.md) — what the search stops on.
- [EpsilonLadderFinder](EpsilonLadderFinder.md) — runs it with weighted heuristics.
- [IGraph](../../../Domain/Core/IGraph.md) — what it searches.
