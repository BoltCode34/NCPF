# PlanGeometry

Turns a searched chain of edges back into real curves.

`NCPF.Pipeline.Application` · `PlanGeometry.cs`

## Description

During the search, an edge carries no shape — only where it leads, what it costs, and which primitive it is ([TransitionData](../../../Domain/Space/TransitionData.md)). Carrying shapes would mean creating one for every edge of every node explored and then discarding almost all of them.

PlanGeometry creates the shapes afterwards, for the winning path only. A primitive is stored as it looks at the origin, so rebuilding an edge's curve is the stored primitive moved to the node the edge starts from.

## Constructor

```csharp
PlanGeometry(DiscretizedControlSet3D controlSet, IGridMap3D grid)
```

| Parameter | Purpose |
|---|---|
| `controlSet` | Resolves a primitive id into its stored curve. |
| `grid` | Converts the edge's start node into the world offset to move the curve by. |

## Key API

```csharp
CurvateTransition[] Resolve(TransitionData[] plan)
```

Returns one curve per edge, in the same order, each placed at its start node. Returns an empty array if `plan` is empty or if any primitive id cannot be resolved.

## Related

- [TransitionData](../../../Domain/Space/TransitionData.md) — the edges it resolves.
- [PathOffset](../Space/PathOffset.md) — how a curve is moved into place.
- [PathFindingAgent](PathFindingAgent.md) — resolves each search result.
