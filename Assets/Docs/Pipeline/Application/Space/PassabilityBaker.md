# PassabilityBaker

Checks every curve of the control set against the map at every lattice pose, once, and stores the answers as a bit mask.

`NCPF.Pipeline.Application` · `PassabilityBaker.cs`

## Description

Whether a curve fits at a pose depends only on the pose and the obstacles, and neither changes during a search. Without baking, the search would re-check the same curves at the same poses thousands of times.

For each heading layer and each free cell, the baker takes the cells each curve of that layer passes through (recorded by [DiscretizedControlSet3DConverter](../Search/DiscretizedControlSet3DConverter.md)), moves them to the cell, and checks that all of them are free for the body. Blocked poses are skipped entirely: the search never expands them.

The result is a [PrimitivePassability](../../../Domain/Space/PrimitivePassability.md): one bit per pose and curve.

Baking runs when the planner builds its map, and costs time proportional to *cells × headings × curves × curve length*. With [dependency sharing](../../../Pipeline/Presentation/NcpfPlannerBootstrap.md), agents that use the same map and control set bake it once.

## Key API

```csharp
PrimitivePassability Bake(IGridMap3D grid, DiscretizedControlSet3D controlSet)
```

| Parameter | Purpose |
|---|---|
| `grid` | The lattice with obstacles. |
| `controlSet` | The curves with their recorded cells. |

## Notes

- **Rebake after changing obstacles.** The mask is a snapshot. Moving obstacles are not supported by it.

## Related

- [PrimitivePassability](../../../Domain/Space/PrimitivePassability.md) — the result.
- [PassabilityTransitionGenerator](PassabilityTransitionGenerator.md) — reads it during the search.
