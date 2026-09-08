# ControlSet

The geometric primitive library: layers of paths keyed by the START direction index only — speed is a dimension of the search graph, not of the shape library.

## Details

- `LoadLayer` appends a layer for an angle index; `GetLayerSet`/`GetLayers` read them back. `MaxAngle`/`AngleCount` describe the baked range.
- Only a quarter of the lattice is baked; the remaining quadrants are rebuilt on read (see `QuarterSymmetry`).
- `DiscretizedControlSet3D` is the `ControlSet<DiscretizedPath3D>` specialisation the search graph consumes.

## Used by

`ControlSetBuilder` fills it on the Bake side; `FuncControlSetContainer` persists it; `Projector3D` reads projected layers at search time.
