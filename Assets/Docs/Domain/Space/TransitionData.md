# TransitionData

A search edge of the curve lattice, reduced to what the search reads: where it goes, what it costs, and which curve it is.

`NCPF.Domain` · `TransitionData.cs`

## Description

```csharp
public struct TransitionData : ITransition
{
    int Start;
    int PrimitiveId;
    int Neightbor;
    float Weight;
}
```

The search creates and discards enormous numbers of edges and only reads their destination and weight. So an edge carries no shape: the curve is named by its id in the [ControlSet](../ControlSet/ControlSet.md), and the shape is recovered only for the path that wins, by [PlanGeometry](../../Pipeline/Application/Search/PlanGeometry.md).

`Start` makes the edge self-contained: the curve id plus the node it leaves from is all that is needed to rebuild the curve in place. The struct takes 16 bytes.

| Field | Meaning |
|---|---|
| `Start` | The node id the edge leaves from. |
| `PrimitiveId` | The curve's id in the control set. |
| `Neightbor` | The node id the edge leads to. |
| `Weight` | The edge's cost. |

## Related

- [PlanGeometry](../../Pipeline/Application/Search/PlanGeometry.md) — turns edges back into curves.
- [ICurvateGraph](../../Pipeline/Application/Space/ICurvateGraph.md) — the graph that returns them.
