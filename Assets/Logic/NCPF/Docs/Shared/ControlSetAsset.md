# ControlSetAsset

The abstract ScriptableObject port a baked control set is stored through. Inspector fields are typed by this base, so every assembly works with the port while Unity binds whatever concrete asset the project uses.

## Details

- `Read(IGridGeometry3D)` rebuilds the full `ControlSet<IPath>` (all four quadrants); `ReadLayer(int, IGridGeometry3D)` returns a single heading layer; `Write` stores pure geometry — coefficients and end nodes, no dynamics.
- Pure geometry both ways: dressing the shapes with dynamics is the Pipeline assembly's business, the port never sees it.

## Used by

`FuncControlSetContainer` implements it; `PathPlanner` and `ControlSetBaker` hold their containers through this port.
