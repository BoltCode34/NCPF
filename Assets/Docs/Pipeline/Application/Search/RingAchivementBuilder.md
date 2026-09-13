# RingAchivementBuilder

An achievement that accepts any node in a ring around the target: closer than a maximum distance, but no closer than a minimum.

`NCPF.Pipeline.Application` · `RingAchivementBuilder.cs`

## Description

Use it when the agent should approach and keep a distance, such as circling prey or stopping short of a door. The goal is a whole region, and the search picks whichever entry into the ring is cheapest to reach, accounting for walls and turning.

Distances are measured in metres between world positions, from the target cell's centre. Nodes inside the inner radius do not count as arrival. Heading is ignored.

Used by `RingAchivementFeature`.

## Constructor

```csharp
RingAchivementBuilder(IGridMap3D map3D, float minDistance, float maxDistance)
```

| Parameter | Purpose |
|---|---|
| `map3D` | Converts nodes to world positions to measure distance. |
| `minDistance` | The inner radius, in metres. |
| `maxDistance` | The outer radius, in metres. If smaller than `minDistance`, the two are swapped. |

## Key API

```csharp
IAchivementHandler Create(Config start, Config end)
```

Returns an achievement for the ring around `end`.

## Notes

- **Keep the ring wide.** A ring narrower than a couple of cells may contain no node the lattice can reach, and the search then finds nothing.
- The standard heuristic still aims at the target's centre, so it overestimates by up to the outer radius. That makes the search a little greedier, which is acceptable with the [epsilon ladder](EpsilonLadderFinder.md).

## Related

- [PointCellAchivementBuilder](PointCellAchivementBuilder.md) — reach the target's cell instead.
- [IGoal](IGoal.md) — the achievement contract.
- [Planner features](../../../Pipeline/Presentation/PlannerFeatures.md) — choosing an achievement.
