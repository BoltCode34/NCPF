# IGridMap3D

The full contract of the 3D lattice the search runs on: lattice enumeration, world↔cell geometry, id coding and graph adjacency.

## Details

- `IGridLattice3D` — (cell x, cell y, direction) nodes: free/blocked queries, packed id coding, an `IDirectGraph`.
- `IGridGeometry3D` — cell size, direction layer step, world↔cell quantisation (extends the 2D grid geometry).
- `IGraphCoder3D` — encodes/decodes between a node id and a world pose (x, y, heading); the live seam the game-side agents call through.

## Used by

`SpacedMap3D` implements it over a baked `Map3D`; `ChainCurvateAgent`, `PathFindingAgent` and `PathPlanner` code poses to ids through it.
