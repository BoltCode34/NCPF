# SpacedMap3D

Adapts a baked `Map3D` to the `IGridMap3D` lattice contract: world↔cell quantisation, id coding and graph adjacency over the baked C-space.

## Details

- Bounds are checked before id packing — past the edge, `CoordToId` wraps onto the opposite side of the map, so out-of-range cells must read as blocked.
- Angles are wrapped into `[0, AngleCount)` for any input, negative or past a full turn, so every id matches a lattice node.

## Used by

The shared seam between baked data and the search: `PathPlanner` and `ChainCurvateAgent` code poses through it.
