# IGoal

What a search is looking for: a test for arrival, and an estimate of how far away arrival still is.

`NCPF.Pipeline.Application` · `PathFinder3D.cs`

## Description

NCPF searches do not aim at a node. They aim at a goal, which is two independent parts:

```csharp
public interface IAchivementHandler { bool Achieved(int point); }
public interface IHeuristicHandler  { float Heuristic(int point); }
public interface IGoal : IAchivementHandler, IHeuristicHandler { }
```

- **Achievement** answers whether a node counts as arrival. It can accept one cell, any heading in a cell, or a whole region.
- **Heuristic** estimates the remaining cost from a node. It steers the search; a better estimate means fewer nodes expanded.

The two parts are kept separate on purpose. Combining them into goal classes would need a class for every pairing of achievement and heuristic. `GoalBase` pairs any two at runtime:

```csharp
GoalBase(IAchivementHandler achivementHandler, IHeuristicHandler heuristicHandler)
```

Pairing them sensibly is the caller's responsibility: a heuristic that aims at the centre of a ring still works with a ring achievement, but overestimates by up to the ring's radius.

### Writing a heuristic

Use the same units as the graph's edge weights. On the lattice of curves, an edge weight is travel time, so a heuristic must estimate **time** — for example, distance divided by top speed.

For the search to return the cheapest path, the estimate must never exceed the real remaining cost.

## Cost handlers

`ICostHandler`, `ICostedGoal` and `CostedGoalBase` describe a goal that also adds a cost per node. [GoalPathFinderBase](GoalPathFinderBase.md) does not read it. To make nodes cost more, decorate the graph's weights instead — `MotionRulesGraph` in [MotionWindow](../Agent/MotionWindow.md) does exactly this with an `ICostHandler`.

## Related

- [IGoalBuilder](IGoalBuilder.md) — creates goals.
- [GoalPathFinderBase](GoalPathFinderBase.md) — searches for them.
- [PointCellAchivementBuilder](PointCellAchivementBuilder.md), [RingAchivementBuilder](RingAchivementBuilder.md) — achievements.
- [DirectDistHeuristicsBuilder](DirectDistHeuristicsBuilder.md), [DijkstraHeuristicBuilder](DijkstraHeuristicBuilder.md) — heuristics.
