# Unity2DGrid

A component that lays a grid over part of the scene and records which cells are blocked by obstacles. Both baking and planning use it to convert between world positions and cells.

`NCPF.Shared.Presentation` · `Unity2DGrid.cs`

## Description

The grid starts at the GameObject's position and extends along +X and +Y for **World Size** metres, divided into cells of **Cell Size** metres. Cell `(0, 0)` is centred on the GameObject's position.

To find obstacles it uses 2D physics: a cell is blocked when a Collider2D on one of the **Obstacle Mask** layers overlaps a box 90% the size of the cell, centred on it.

The grid itself knows nothing about the agent's shape. [NCPFMapBuilder](../Bake/Presentation/NCPFMapBuilder.md) turns these blocked cells into a map of where the body fits.

## Inspector

| Field | Purpose |
|---|---|
| **World Size** | Width and height of the covered area, in metres. |
| **Cell Size** | Size of one cell, in metres. |
| **Obstacle Mask** | The layers whose colliders count as obstacles. |
| **Bake On Awake** | Reads the obstacles when the scene starts. Leave it enabled. |
| **Draw Gizmos** | Draws the grid in the Scene view: free cells cyan, blocked cells red. |

## Key API

```csharp
void Bake()
```

Reads the obstacles again from the physics scene. Call it after moving obstacles or changing the grid's settings.

```csharp
Vector2 CellToWorld(Int2 cell)
Int2 WorldToCell(Vector2 worldPosition)
```

Convert between a cell and the world position of its centre.

```csharp
bool CellFree(int x, int y)
```

Returns `true` if no obstacle overlaps the cell. Cells outside the grid count as blocked.

```csharp
Int2 Size { get; }
float CellSize { get; }
```

The grid's size in cells, and the size of one cell in metres.

## Notes

- **Turn Draw Gizmos off when you are done checking.** Drawing re-reads the obstacles every time the Scene view redraws, which is wasteful, and in Play mode it can race with searches reading the grid.
- **Bake uses physics queries,** so it must run on the main thread with the obstacle colliders enabled.
- The grid can also be searched as a plain 2D graph through `PointFree` and `GetNeightbors`. That graph is created once and keeps reading the data from the first bake; call `Bake` before first use and do not rely on it after re-baking. NCPF's own planning does not use it.

## Related

- [NCPFMapBuilder](../Bake/Presentation/NCPFMapBuilder.md) — bakes the map from this grid.
- [SpacedMap3D](SpacedMap3D.md) — combines this grid with a baked map for the search.
- [Getting Started](../GettingStarted.md) — step 1 sets one up.
