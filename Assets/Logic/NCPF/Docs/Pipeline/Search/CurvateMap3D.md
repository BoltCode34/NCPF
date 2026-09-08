# CurvateMap3D

The geometric search graph. By node id it yields neighbors — [[CurvateTransition]] edges carrying primitive shapes. There is no velocity in it: this is pure `(x, y, direction)` geometry.

`NCPF.Pipeline.Application · Space/CurvateMap3D.cs`

## What it is

A thin glue of two things: the node space (an [[IGridMap3D]]) and an edge generator ([[ITransitionGenerator]]). Almost no logic of its own — enumerating neighbors and pricing them is entirely the generator's business. The graph knows neither the projectors nor where the weights come from.

Named for the voxel collision `Map3D` in bake (its namesake); this is the graph, not the map.

## Constructor

```csharp
CurvateMap3D(IGridMap3D grid, ITransitionGenerator generator)
```

`grid` — the node space: translates id ↔ cell and answers whether a cell is free. Everything about "where we are" comes from it.

`generator` — the edge generator: by a cell it yields weighted primitives. Everything about "where we can go" comes from it. Usually a [[CurvateEdgeGenerator]] over a validator-wrapped [[Projector3D]].

## Key API

```csharp
CurvateTransition[] GetNeightbors(int id)
```
Neighbors of a node — the edges out of it. Asks the generator, after translating the id to a cell.

**Args** `id` — a graph node

**Return** array of [[CurvateTransition]]: neighbor + primitive geometry + weight

```csharp
bool PointFree(int id)
```
Whether the node is passable — whether the body fits in that cell with that heading.

**Args** `id` — a graph node

**Return** `true` if the node is passable

## Used by

The search ([[EpsilonLadderFinder]], [[PathFindingAgent]]) walks this graph through the generic finder [[CurvateGoalPathFinder]]. The rope agent [[ChainCurvateAgent]] wraps it in a [[RearGearGraph]] for its own search, but the graph knows nothing about the rope.

## Related

- [[IGridMap3D]] — the node space
- [[ITransitionGenerator]] · [[CurvateEdgeGenerator]] — edges and their weights
- [[ICurvateGraph]] — the contract it implements
- [[CurvateTransition]] — the edge
- [[RearGearGraph]] — decorator that re-weights
- [[PathFindingAgent]] — where it is used