# CurvateMap3D

The base search graph over the lattice. It answers whether a node is free and delegates everything about edges to a transition generator.

`NCPF.Pipeline.Application` · `CurvateMap3D.cs`

## Description

CurvateMap3D is deliberately thin. It converts a node id to a cell and heading, then:

- asks the map whether that pose is free;
- asks its [ITransitionGenerator](ITransitionGenerator.md) for the edges leaving it.

Which curves exist at a node and what they cost is entirely the generator's business. To change how edges are produced, replace the generator, not the graph.

## Constructor

```csharp
CurvateMap3D(IGridMap3D grid, ITransitionGenerator generator)
```

| Parameter | Purpose |
|---|---|
| `grid` | The lattice: id conversion and which poses are free. |
| `generator` | Produces the edges of a node. |

## Key API

```csharp
TransitionData[] GetNeightbors(int id)
bool PointFree(int id)
```

## Related

- [ICurvateGraph](ICurvateGraph.md) — the contract and the standard stack.
- [PassabilityTransitionGenerator](PassabilityTransitionGenerator.md) — the standard generator.
