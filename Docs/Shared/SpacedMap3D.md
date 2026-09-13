# SpacedMap3D

The search's view of the world: a baked map placed on a scene grid. It answers where a lattice node is in the world, which node a world pose belongs to, and whether the body fits at a node.

`NCPF.Shared.Presentation` · `SpacedMap3D.cs`

## Description

A baked [Map3D](../Domain/Collision/Map3D.md) knows which cells are free at each heading, but not where those cells are in the scene. A [Unity2DGrid](Unity2DGrid.md) knows where cells are, but not where the body fits. SpacedMap3D combines the two into an [IGridMap3D](../Domain/Space/IGridMap3D.md):

- positions, cell size and grid size come from the grid;
- occupancy, heading count and heading step come from the map.

## Constructor

```csharp
SpacedMap3D(IGridMap2D grid, Map3D map)
```

| Parameter | Purpose |
|---|---|
| `grid` | Places the cells in the world. Must be the grid the map was baked on. |
| `map` | Says where the body fits at each heading. |

## Key API

```csharp
Config WorldToCell3D(WorldConfig config)
WorldConfig Cell3DToWorld(Config config)
```

Convert between a world pose and a lattice node. Any heading is wrapped into the map's range, so a pose always lands on a valid node.

```csharp
bool Cell3DFree(int x, int y, int angle)
```

Returns `true` if the body fits at that node. Nodes outside the map count as blocked.

```csharp
int CellToId(int x, int y, int angle)
Config IdToCell3D(int id)
```

Convert between a node and the integer id the search uses.

```csharp
float CellSize { get; }
float AngleStep { get; }
int AngleCount { get; }
```

The size of a cell in metres, the angle between neighbouring headings in degrees, and the number of headings.

## Notes

- **Check bounds before packing an id.** Ids have no gaps, so an out-of-range coordinate produces a valid id of a different node. `Cell3DFree` checks bounds for you; if you build ids yourself, check first.

## Related

- [IGridMap3D](../Domain/Space/IGridMap3D.md) — the contract it implements.
- [Map3D](../Domain/Collision/Map3D.md) — the baked occupancy.
- [Unity2DGrid](Unity2DGrid.md) — the scene grid.
