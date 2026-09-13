# IGridMap3D

The lattice the search runs on: cells, headings, which poses are free, and how poses convert to world positions and node ids.

`NCPF.Domain` · `IGridMap3D.cs`, `IGridLattice3D.cs`, `IGraphCoder3D.cs`

## Description

IGridMap3D combines three roles, each its own interface:

| Interface | Role |
|---|---|
| `IGridLattice3D` | The discrete side: size, heading count, whether a pose is free, and conversion between a [Config](../Core/Config.md) and a node id. |
| `IGridGeometry3D` | The world side: cell size, heading step, and conversion between a Config and a [WorldConfig](../Core/WorldConfig.md). |
| `IGraphCoder3D` | Direct conversion between node ids and world poses. |

Code that only needs one role should take only that interface. [QuarterSymmetry](../ControlSet/QuarterSymmetry.md), for example, needs only `IGridGeometry3D`.

The standard implementation is [SpacedMap3D](../../Shared/SpacedMap3D.md): a baked [Map3D](../Collision/Map3D.md) placed by a [Unity2DGrid](../../Shared/Unity2DGrid.md).

## Key API

```csharp
Int2 Size { get; }
int AngleCount { get; }
bool Cell3DFree(Config config)
bool Cell3DFree(int x, int y, int angle)
```

The size in cells, the number of headings, and whether the body fits at a pose.

```csharp
int Cell3DToId(Config config)
int CellToId(int x, int y, int angle)
Config IdToCell3D(int id)
```

Converts between poses and node ids.

```csharp
float CellSize { get; }
float AngleStep { get; }
WorldConfig Cell3DToWorld(Config config)
Config WorldToCell3D(WorldConfig config)
```

Converts between lattice poses and world poses. A world pose is rounded to the nearest heading.

## Related

- [SpacedMap3D](../../Shared/SpacedMap3D.md) — the standard implementation.
- [Config](../Core/Config.md), [WorldConfig](../Core/WorldConfig.md) — the pose types.
