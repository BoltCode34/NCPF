# IAsyncGoalPathFinder

The contract for a path finder that can run in the background.

`NCPF.Pipeline.Application` · `IAsyncGoalPathFinder.cs`, `PathFinder3D.cs`

## Description

```csharp
public interface IGoalPathFinder<T> where T : ITransition
{
    T[] FindPath(IGraph<T> graph, int from, IGoal goal, int maxIterations = 1000);
}

public interface IAsyncGoalPathFinder<T> : IGoalPathFinder<T> where T : ITransition
{
    IPathFindingTask<T> FindPathAsync(IGraph<T> graph, int from, IGoal goal, int maxIterations = 1000);
}
```

An asynchronous finder can always run synchronously too. `FindPathAsync` returns immediately with a task the caller polls or awaits.

## IPathFindingTask

```csharp
TaskState State { get; }
T[] Result { get; }
TaskAwaiter<T[]> GetAwaiter()
void Break()
```

| Member | Meaning |
|---|---|
| `State` | `InProccess` while running, `Completed` when done, `Canceled` after a cancellation took effect. |
| `Result` | The path, once the task has completed successfully. `null` until then. |
| `GetAwaiter` | Lets the task be awaited. |
| `Break` | Asks the task to stop. |

## Notes

- **A failed task reports `Completed`.** If the search throws, `State` becomes `Completed` and `Result` stays `null`, so it cannot be told apart from a search that found nothing by looking at these two members.

## Related

- [EpsilonLadderFinder](EpsilonLadderFinder.md) — the standard implementation.
- [GoalPathFinderBase](GoalPathFinderBase.md) — the standard synchronous finder.
