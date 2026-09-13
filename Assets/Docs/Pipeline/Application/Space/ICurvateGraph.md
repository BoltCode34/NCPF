# ICurvateGraph

The search graph of curves: nodes are poses on the lattice, edges are the control set's curves.

`NCPF.Pipeline.Application` · `ICurvateGraph.cs`

## Description

```csharp
public interface ICurvateGraph : IGraph<TransitionData> { }
```

A node is a lattice pose, a cell and a heading, packed into one integer id. An edge from a node is one curve of the control set that fits there, leading to the pose where the curve ends.

The graph is purely geometric. It carries no speed and no time, except that edge **weights** are travel-time estimates. Speed is planned later, along the chosen path.

Describing weights is the graph's job, which is what makes decoration possible: a decorator wraps a graph and changes the weights it reports. The standard graph is a stack:

| Layer | Role |
|---|---|
| [CachedCurvateGraph](CachedCurvateGraph.md) | Remembers each node's edges. Always outermost. |
| [RearGearGraph](RearGearGraph.md) | Makes reversing more expensive. |
| [CurvateMap3D](CurvateMap3D.md) | The graph itself. |
| [PassabilityTransitionGenerator](PassabilityTransitionGenerator.md) | Produces and prices the edges for `CurvateMap3D`. |

[NcpfGraphComposer](../../../Pipeline/Presentation/NcpfGraphComposer.md) builds this stack.

## CurvateTransition

```csharp
public struct CurvateTransition : ITransition
{
    IPath Path;
    int Neightbor;
    float Weight;
}
```

An edge **with** its shape. The search itself uses the thin [TransitionData](../../../Domain/Space/TransitionData.md); `CurvateTransition` is what [PlanGeometry](../Search/PlanGeometry.md) produces for the finished path, and what the driving code follows.

| Field | Meaning |
|---|---|
| `Path` | The curve in world space. |
| `Neightbor` | The node the curve ends at. |
| `Weight` | The edge's cost. |

## Writing a decorator

Wrap an `ICurvateGraph`, forward both members, and change the weights of the returned edges. Place your decorator **below** [CachedCurvateGraph](CachedCurvateGraph.md): if it sits above and modifies the cached array, the change accumulates every time the node is visited.

## Related

- [IGraph](../../../Domain/Core/IGraph.md) — the base contract.
- [NcpfGraphComposer](../../../Pipeline/Presentation/NcpfGraphComposer.md) — builds the standard stack.
- [GoalPathFinderBase](../Search/GoalPathFinderBase.md) — searches it.
