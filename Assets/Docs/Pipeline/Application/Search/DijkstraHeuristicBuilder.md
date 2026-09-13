# DijkstraHeuristicBuilder

An accurate heuristic that knows about walls: a precomputed distance field from the target over the whole lattice.

`NCPF.Pipeline.Application` · `DijkstraHeuristicBuilder.cs`, `DijkstraField3D.cs`, `DijkstraHeuristic3D.cs`

## Description

The builder runs Dijkstra's algorithm backwards from the target cell over every cell and heading, producing a `DijkstraField3D`: the distance to the target from anywhere, going around obstacles. The heuristic reads that distance and divides it by the agent's top speed to get a time.

The field uses a simplified model of motion, not the real curves: from a node you may step one cell forward or backward along the current heading (diagonal steps cost √2 cells), or turn on the spot by one heading step for `turnPenaltyCells` cells of distance. Nodes the field cannot reach fall back to the straight-line distance.

Because the field already accounts for walls, the search heads straight along the real route instead of exploring dead ends. In a cluttered level this saves far more than the field costs.

Building the field sweeps the whole map, so the builder keeps it and builds a new one **only when the target moves to a different cell**. It builds on a background thread through the asynchronous build path.

Used by `DijkstraHeuristicFeature`.

## Constructor

```csharp
DijkstraHeuristicBuilder(IGridMap3D map3D, float turnPenaltyCells, float maxSpeed)
```

| Parameter | Purpose |
|---|---|
| `map3D` | The lattice the field is computed over. |
| `turnPenaltyCells` | The cost of turning on the spot by one heading step, as a distance in cells. Higher values make the estimate prefer straight routes. |
| `maxSpeed` | The agent's top speed, to turn distance into time. |

## Key API

```csharp
IHeuristicHandler Create(Config start, Config end)
```

Returns a heuristic measuring toward `end`, reusing the field if `end` is in the same cell as last time.

## Notes

- **Not guaranteed to underestimate.** The simplified motion model does not match the curves exactly, so on some nodes the estimate can exceed the real cost, and the path found may not be the cheapest. In practice it behaves like a mildly weighted heuristic.
- **One instance per agent.** The builder caches its field for one caller on one thread. Sharing a builder between agents that search at the same time is not safe.
- Moving targets that change cell often cause frequent rebuilds. For a target that moves constantly, measure whether the rebuild costs more than it saves.

## Related

- [DirectDistHeuristicsBuilder](DirectDistHeuristicsBuilder.md) — the cheap alternative.
- [IGoal](IGoal.md) — the heuristic contract.
- [Planner features](../../../Pipeline/Presentation/PlannerFeatures.md) — choosing a heuristic.
