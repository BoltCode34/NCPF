# Map3D

The occupancy lattice over (x, y, heading): a walkability boolean per cell per baked direction layer, plus the agent rig the layers were convolved with. Implements `IDirectGraph` over packed cell ids.

## Details

- `GetNeightbors` walks the 26-neighbourhood; the direction index wraps around (z is cyclic), x/y clamp at the world edges.
- `InBounds` must be asked before `CoordToId`: the id packing has no digit gaps, so an out-of-range x decodes to the opposite edge — a valid id of the wrong cell that `PointFree` can no longer catch.
- `AgentRig` is the snapshot of the footprint the map was baked with: equal footprints can still mean different maps when the body is towed by a different point (the anchor differs).

## Used by

`MapBuilder` bakes it; `SpacedMap3D` adapts it into the `IGridMap3D` contract the search runs on.
