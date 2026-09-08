# DiscretizedPath3D

A pure geometric shape plus the lattice cells it sweeps: the primitive form the 3D search graph trades in. Cells are `Config` (x, y, direction) — there is no velocity anywhere, in the type or in the data.

## Details

- Collision reads the sweep (`IdSequence`), the search reads the geometry, dynamics are layered on only after the search.
- Implements `IPathWrapper`, so wrapper-aware logic (`PathGearUtility`) can look through it to the raw shape.
- `DiscretizedControlSet3D` is a `ControlSet<DiscretizedPath3D>`: the discretized primitive library the graph is built from.

## Used by

`DiscretizedControlSet3DConverter` produces these from a geometric control set; `CurvateTransition` carries one per graph edge.
