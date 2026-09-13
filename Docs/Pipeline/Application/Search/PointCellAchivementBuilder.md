# PointCellAchivementBuilder

The standard achievement: the target counts as reached when the agent is in the target's cell, at any heading.

`NCPF.Pipeline.Application` · `PointCellAchivementBuilder.cs`, `PointCellAchivement3D.cs`

## Description

Heading is ignored on purpose. Requiring an exact heading at the target would make many targets unreachable, because not every heading can be arrived at through the curves in the control set. Which way the agent faces once it arrives is decided by the passes, not by the search.

Used by `PointCellAchivementFeature`, the standard achievement feature.

## Constructor

```csharp
PointCellAchivementBuilder(IGridMap3D map3D)
```

| Parameter | Purpose |
|---|---|
| `map3D` | Converts nodes to cells to compare them. |

## Key API

```csharp
IAchivementHandler Create(Config start, Config end)
```

Returns an achievement that accepts any node in `end`'s cell.

## Related

- [RingAchivementBuilder](RingAchivementBuilder.md) — reach a distance around the target instead.
- [IGoal](IGoal.md) — the achievement contract.
- [Planner features](../../../Pipeline/Presentation/PlannerFeatures.md) — choosing an achievement.
