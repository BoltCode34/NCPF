# DiscretizedControlSet3DConverter

Prepares a control set for the search by recording which lattice cells each curve passes through.

`NCPF.Pipeline.Application` · `DiscretizedControlSet3DConverter.cs`

## Description

To know whether a curve fits at a node, you need the cells the body occupies along it. The converter walks each curve by distance, in small steps, and records every distinct cell and heading it lands in. The result, a [DiscretizedControlSet3D](../../../Domain/ControlSet/DiscretizedControlSet3D.md), pairs every curve with that list of cells.

The walk is purely by distance: curves have no time or speed, so there is nothing else to account for.

The recorded cells are what [PassabilityBaker](../Space/PassabilityBaker.md) checks against the map.

## Key API

```csharp
DiscretizedControlSet3D Create(IGridGeometry3D space, ControlSet<IPath> controlSet)
```

| Parameter | Purpose |
|---|---|
| `space` | The grid whose cells are recorded. |
| `controlSet` | The curves to walk. |

```csharp
float Step
```

The distance between samples, in metres. Default `0.05`. A step larger than a cell can miss cells a curve only clips.

## Related

- [DiscretizedControlSet3D](../../../Domain/ControlSet/DiscretizedControlSet3D.md) — what it produces.
- [PassabilityBaker](../Space/PassabilityBaker.md) — uses the recorded cells.
