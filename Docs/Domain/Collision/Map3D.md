# Map3D

The baked occupancy of the level for one body shape: for every cell and every heading, whether the body fits there.

`NCPF.Domain` · `Map3D.cs`, `AgentRig.cs`

## Description

A long body can fit through a corridor lengthwise but not sideways. So "is this cell free" depends on which way the body faces. Map3D stores the answer for every cell at every heading, as a 3D array `[x, y, heading]`: `true` where the body fits.

It is produced by [MapBuilder](MapBuilder.md), saved and loaded by [MapContainer](../../Config/MapContainer.md), and placed in the scene by [SpacedMap3D](../../Shared/SpacedMap3D.md), which is what the rest of the package uses.

It also keeps the `AgentRig` it was baked with: the body's footprint, the cell size, and the anchor cell. This records which shape the map is for.

Map3D is a graph as well: each node connects to its 26 neighbours in cells and headings, with the heading wrapping around. This plain graph is not used by the curve search.

## Members

| Member | Meaning |
|---|---|
| `Map` | The occupancy, `[x, y, heading]`, `true` = free. |
| `SizeZ` | The number of headings. |
| `AngleStep` | Degrees between headings. |
| `CellSize` | The cell size the map was baked with. |
| `Rig` | The body it was baked for. |
| `InBounds(x, y)` | Whether a cell is on the map. |
| `CoordToId(x, y, z)`, `IdToCoord(id, …)` | Converts between a cell and heading and a node id. |
| `PointFree(id)` | Whether the body fits at a node. |

## Notes

- **Check bounds before creating an id.** `CoordToId` does not check its input, and an off-map coordinate produces the id of a different, valid cell. Call `InBounds` first.

## AgentRig

```csharp
AgentRig(bool[,] footprint, float cellSize, Int2 anchor = default)
```

| Field | Meaning |
|---|---|
| `Footprint` | The body's cells at heading 0. |
| `CellSize` | The cell size of the footprint. |
| `Anchor` | The footprint cell the body is placed by. |

## Related

- [MapBuilder](MapBuilder.md) — bakes it.
- [SpacedMap3D](../../Shared/SpacedMap3D.md) — places it in the scene.
- [NCPFMapBuilder](../../Bake/Presentation/NCPFMapBuilder.md) — bakes it from the Editor.
