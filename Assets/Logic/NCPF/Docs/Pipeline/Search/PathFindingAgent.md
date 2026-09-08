# PathFindingAgent

The geometric search: builds a path (async, ε-ladder), buffers it, and on demand stitches it to the agent's current position. Dynamics-free — pure [[CurvateTransition]] geometry. The goal builds by the async contract ([[IAsyncGoalBuilder]] — a heavy heuristic goes off-thread); the search itself runs in the injected finder.

`NCPF.Pipeline.Application · Search/PathFindingAgent.cs`

## Constructor

```csharp
PathFindingAgent(IGridMap3D map3D,
                 ICurvateGraph graph,
                 IAsyncGoalPathFinder<CurvateTransition> pathFinder,
                 IGoalPathFinder<CurvateTransition> stitchFinder,
                 IAsyncGoalBuilder goalBuilder,
                 DynamicAgent dynamicAgent,
                 int attemptBudget)
```

`map3D` — the lattice: world↔cell/id, free queries. Used for the start/end cells and the stitch.

`graph` — the [[ICurvateGraph]] searched (usually [[CurvateMap3D]] wrapped by [[RearGearGraph]]).

`pathFinder` — the async finder (usually [[EpsilonLadderFinder]]) for the main search.

`stitchFinder` — the synchronous finder ([[CurvateGoalPathFinder]]) for the short stitch between the old plan's current primitive and the new plan.

`goalBuilder` — builds the goal asynchronously; a heavy heuristic (Dijkstra field) builds off-thread.

`dynamicAgent` — the limits ([[DynamicAgent]]); `MaxVelocity` feeds the stitch goal's heuristic so its units match the edge weights.

`attemptBudget` — iterations per search rung.

## Key API

```csharp
void BuildPath(DynamicState start, DynamicState end)
```
Starts the search. The goal builds asynchronously; the `_goalBuilding` flag is set synchronously **before** the first await, so the `InProcess` guard sees it immediately — no double sweep. The continuation lands on the main thread (Unity context), so the task slot is only ever mutated on the main thread.

```csharp
CurvateTransition[] StitchPath(CurvateTransition[] oldPath, int currentPart)
```
Takes the buffered result and stitches it to the old plan jump-free. Only the current primitive of the old plan stays (the body is on it); its end is the anchor — a lattice node the body is guaranteed to pass. Then a short sync stitch to the primitive **following** the one nearest the anchor on the new path, then the tail of the new path from there. If the stitch fails the result is discarded (raw application would teleport the body back). The search result is consumed here — the task is released so it is not stitched twice.

```csharp
bool HasBufferedPath / bool InProcess / bool HasTask
```
State flags for the brain's loop. `InProcess` covers both the goal build and the search itself. `HasBufferedPath` is true when a result is ready and not yet consumed.

```csharp
void Break()
```
Releases the in-flight task and bumps the generation, so a late continuation from an old search is ignored.

## Used by

The brain [[PathAgentPipeline]] owns one, feeds it the body state each tick, and consumes buffered results via `StitchPath`.

## Related

- [[PathAgentPipeline]] — the brain that owns it
- [[EpsilonLadderFinder]] — the usual pathFinder
- [[CurvateGoalPathFinder]] — the usual stitchFinder
- [[IAsyncGoalBuilder]] · [[BaseGoalBuilder]] — the goal contract
- [[CurvateTransition]] — the edge it builds with
- [[ICurvateGraph]] — the graph it searches