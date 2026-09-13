# DirectDistHeuristicsBuilder

The standard heuristic: the straight-line distance to the target, divided by the agent's top speed.

`NCPF.Pipeline.Application` · `DirectDistHeuristicsBuilder.cs`

## Description

Edge weights on the lattice are travel times, so the estimate is a time too: how long the remaining distance would take in a straight line at full speed. No real path is shorter or faster than that, so the estimate never overestimates and the search stays correct.

It costs almost nothing to compute and needs no preparation. The trade-off is that it ignores walls, so in a maze-like level the search explores many dead ends. [DijkstraHeuristicBuilder](DijkstraHeuristicBuilder.md) is the accurate alternative.

Used by `DirectDistHeuristicFeature`, the standard heuristic feature.

## Constructor

```csharp
DirectDistHeuristicsBuilder(IGridMap3D map3D, float maxSpeed)
```

| Parameter | Purpose |
|---|---|
| `map3D` | Converts nodes to world positions to measure distance. |
| `maxSpeed` | The agent's top speed, to turn distance into time. |

## Key API

```csharp
IHeuristicHandler Create(Config start, Config end)
```

Returns a heuristic that measures toward `end`.

## Related

- [IGoal](IGoal.md) — the heuristic contract.
- [DijkstraHeuristicBuilder](DijkstraHeuristicBuilder.md) — the wall-aware alternative.
- [Planner features](../../../Pipeline/Presentation/PlannerFeatures.md) — choosing a heuristic.
