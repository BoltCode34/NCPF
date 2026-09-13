# PathFindingAgent

Runs an agent's path search on a background thread, holds the result until it is needed, and stitches it onto the path the agent is already driving.

`NCPF.Pipeline.Application` · `PathFindingAgent.cs`

## Description

PathFindingAgent does the geometric part of planning only: which chain of curves leads to the target. Speed and facing are decided later, while driving.

A search has two stages, both off the main thread:

1. **Build the goal.** The [IGoalBuilder](IGoalBuilder.md) creates what counts as arrival and how to estimate the remaining cost. An expensive heuristic can take a while here.
2. **Search.** The asynchronous finder runs the search and keeps the result until [PathAgentPipeline](../Agent/PathAgentPipeline.md) asks for it.

The agent reports itself busy for the whole of both stages, so the pipeline never starts a second search on top of the first.

### Stitching

A new path is found from where the agent *was* when the search started, but the agent kept moving. Applying it directly would snap the agent back. `StitchPath` avoids that:

1. It keeps the curve the agent is currently on. That curve's end is a node the agent is guaranteed to reach.
2. On the new path, it finds the curve that starts closest to that node, and takes the one **after** it — the closest one may already be behind the agent.
3. It runs a short search from the node to the start of that curve, reaching the right cell at any heading.
4. The result is the current curve, then the short bridge, then the rest of the new path.

If no bridge can be found, the new path is thrown away rather than applied with a jump.

## Constructor

```csharp
PathFindingAgent(IGridMap3D map3D, ICurvateGraph graph,
                 IAsyncGoalPathFinder<TransitionData> pathFinder,
                 IGoalPathFinder<TransitionData> stitchFinder,
                 IAsyncGoalBuilder goalBuilder, DynamicAgent dynamicAgent,
                 PlanGeometry geometry, int attemptBudget)
```

| Parameter | Purpose |
|---|---|
| `map3D` | Converts world poses to lattice nodes. |
| `graph` | The lattice both searches walk. |
| `pathFinder` | Runs the main search in the background. |
| `stitchFinder` | Runs the short bridge search on the main thread. |
| `goalBuilder` | Creates the goal for each search. |
| `dynamicAgent` | Its top speed turns distance into time for the bridge search's estimate. |
| `geometry` | Turns the searched edges into curves. |
| `attemptBudget` | How many nodes the main search may expand. |

## Key API

```csharp
void BuildPath(DynamicState start, DynamicState end)
```

Starts a search from `start` to `end`. Returns immediately; the search runs in the background. Does nothing while a goal is still being built.

```csharp
CurvateTransition[] StitchPath(CurvateTransition[] oldPath, int currentPart)
```

Takes the finished result and stitches it onto the path being driven.

| Parameter | Purpose |
|---|---|
| `oldPath` | The path the agent is driving. `null` or empty for a cold start. |
| `currentPart` | Index of the curve the agent is on. |

Returns the stitched path, or `null` if there is no result yet or no bridge was found. The result is consumed: a second call returns `null`.

```csharp
void Break()
```

Abandons the current search. A goal still being built is ignored when it finishes.

```csharp
bool InProcess { get; }
bool HasBufferedPath { get; }
bool HasTask { get; }
```

Whether a search is running, whether a finished result is waiting, and whether either is the case.

## Notes

- **Errors are silent.** An exception while building the goal or searching is caught and discarded. From outside, a failed search looks exactly like a search that found no path.
- **Break does not stop a search immediately.** The search is only told to stop between rungs of the [epsilon ladder](EpsilonLadderFinder.md). A rung already running finishes on its thread, while a new search may already be starting.

## Related

- [PathAgentPipeline](../Agent/PathAgentPipeline.md) — decides when to search.
- [EpsilonLadderFinder](EpsilonLadderFinder.md) — the standard background finder.
- [PlanGeometry](PlanGeometry.md) — turns results into curves.
- [IGoalBuilder](IGoalBuilder.md) — builds the goal.
