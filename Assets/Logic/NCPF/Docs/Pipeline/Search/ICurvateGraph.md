# ICurvateGraph

The geometric search graph: nodes are `(x, y, direction)`, edges carry a pure shape. No speed, no time — dynamics are layered on after the search.

`NCPF.Pipeline.Application · Space/ICurvateGraph.cs`

## Contract

```csharp
public interface ICurvateGraph : IGraph<CurvateTransition> { }
```

Adds no members of its own — it takes everything from [[IGraph]]:

| member | what |
|---|---|
| `GetNeightbors(id)` | edges out of a node — an array of [[CurvateTransition]] (neighbor + shape + weight) |
| `PointFree(id)` | whether the node is free (the body fits) |

It is a marker interface: it only fixes that the graph walks **geometric** `CurvateTransition` edges. Because the edge type is concrete, the generic finder [[CurvateGoalPathFinder]] knows what it is working with.

Describing the graph — **including the weights of outgoing transitions** — is the graph's own duty. That is exactly what lets graph decorators (penalties, discounts) modify those weights. A concrete graph may compute the weights itself or delegate the handling ([[CurvateMap3D]] delegates to its [[ITransitionGenerator]]).

## CurvateTransition

```csharp
public struct CurvateTransition : ITransition
{
    public DiscretizedPath3D Path;
    public int Neightbor { get; set; }
    public float Weight { get; set; }
}
```

A geometric edge: a shape ([[DiscretizedPath3D]]) reaching a 3D node, with a weight (search time). The weight is the graph's to own — see [[CurvateEdgeGenerator]].

## Implementations

- **[[CurvateMap3D]]** — the primary: delegates neighbor enumeration and pricing to its [[ITransitionGenerator]].
- **[[RearGearGraph]]** — decorator: same edges, rear-first ones weighted up by a mission penalty.

## Related

- [[IGraph]] — the base generic graph
- [[CurvateTransition]] — the edge type
- [[CurvateMap3D]] — implementation
- [[RearGearGraph]] — decorator
- [[CurvateGoalPathFinder]] — finder over such a graph