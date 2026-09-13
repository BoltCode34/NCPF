# IGraph

The contract every searchable graph in NCPF implements: nodes are integer ids, and each node lists its outgoing edges.

`NCPF.Domain` · `IDirectGraph.cs`

## Description

```csharp
public interface IGraph
{
    ITransition[] GetNeightbors(int id);
    bool PointFree(int id);
}

public interface IGraph<T> : IGraph where T : ITransition
{
    new T[] GetNeightbors(int id);
}
```

A graph answers two questions about a node: where can I go from here, and can I be here at all. Everything else, such as what an id means, is up to the graph.

`IGraph<T>` returns edges as a concrete type, so a search does not have to box or cast them. Implement `IGraph<T>`; the non-generic `GetNeightbors` is provided for you.

## Edges

```csharp
public interface ITransition
{
    int Neightbor { get; }
    float Weight { get; }
}

public struct DirectTransition : ITransition { ... }
```

An edge is its destination and its cost. `DirectTransition` is the plain edge with nothing more. Richer edges add data: [TransitionData](../Space/TransitionData.md) also names the curve it follows.

## Graph kinds

| Interface | Meaning |
|---|---|
| `IGraph` | Any graph. |
| `IGraph<T>` | A graph with edges of type `T`. |
| `IDirectGraph` | A graph with plain `DirectTransition` edges. |
| `ISpacedGraph` | A plain graph whose nodes have 2D world positions. |

## Related

- [GoalPathFinderBase](../../Pipeline/Application/Search/GoalPathFinderBase.md) — searches any `IGraph<T>`.
- [ICurvateGraph](../../Pipeline/Application/Space/ICurvateGraph.md) — the graph of curves.
