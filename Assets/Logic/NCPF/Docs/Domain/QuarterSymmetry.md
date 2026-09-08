# QuarterSymmetry

Quarter-turn (90° CCW) symmetry helpers for the geometric control set: only a quarter of the lattice is baked, the other three quadrants are rebuilt natively on read.

## Details

- A curvature-defined primitive is orientation-invariant: rotating the whole path by q·90° leaves its SHAPE untouched and only rotates the boundary frame — so the rebuild produces a genuine path object (type checks and serialisation keep working, unlike a rotation wrapper).
- NCPF direction is 0 = +Y, +CCW, so one quarter-turn maps a cell (x, y) to (−y, x) and shifts the angle index by a quarter of the full angle count.

## Used by

`FuncControlSetContainer.BuildRotatedLayer` reconstructs the unbaked quadrants on read.
