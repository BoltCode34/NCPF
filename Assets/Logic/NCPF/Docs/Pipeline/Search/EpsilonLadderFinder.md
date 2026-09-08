# EpsilonLadderFinder

An async finder with the ε ladder. Wraps a plain [[IGoalPathFinder]] and gives it a progressively improving search: each rung multiplies the goal's heuristic by ε through [[EpsGoal]]; the first success is returned.

`NCPF.Pipeline.Application · Search/EpsilonLadderFinder.cs`

## Constructor

```csharp
EpsilonLadderFinder(IGoalPathFinder<CurvateTransition> finder, float[] epsilonLadder, int attemptBudget)
```

`finder` — the underlying finder (usually [[CurvateGoalPathFinder]]) that performs the search on one rung.

`epsilonLadder` — the ε rungs, tried in order (e.g. `{ 3, 2, 4, 1.5 }`). A larger ε inflates the heuristic, trading optimality for a faster, broader first sweep; the ladder narrows toward an accurate result.

`attemptBudget` — the iteration budget per rung.

## Key API

```csharp
IPathFindingTask<CurvateTransition> FindPathAsync(IGraph<CurvateTransition> graph, int from, IGoal goal, int maxIterations = 1000)
```
Returns an [[EpsilonLadderTask]] that walks the rungs asynchronously, returning the first path found.

```csharp
CurvateTransition[] FindPath(IGraph<CurvateTransition> graph, int from, IGoal goal, int maxIterations = 1000)
```
The synchronous fallback: a single run on the underlying finder (no ladder).

## Related

- [[CurvateGoalPathFinder]] — the underlying finder
- [[EpsilonLadderTask]] — the rung-walking task
- [[EpsGoal]] — the heuristic-scaling goal wrapper
- [[PathFindingAgent]] — who drives the ladder