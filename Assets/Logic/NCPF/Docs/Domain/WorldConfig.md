# WorldConfig

A continuous pose: position in metres (X, Y) plus heading in degrees (Angle). The continuous counterpart of the discrete lattice node [Config](Config.md).

## Details

- `Position` exposes (X, Y) as a `Vector2`.
- The + and - operators normalize the resulting angle into [0, 360); multiplication by a scalar does not normalize.
- `Normalize` folds any angle into [0, 360); `DeltaAngle` delegates to `Mathf.DeltaAngle`.

## Used by

`IPath` samples, `DynamicState`, and every layer that reasons in world coordinates instead of lattice cells.
