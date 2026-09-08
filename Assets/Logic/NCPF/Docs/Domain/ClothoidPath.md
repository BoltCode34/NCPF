# ClothoidPath

A clothoid (Euler spiral) primitive between two poses: curvature varies linearly with arc length, so the shape is fully described by start curvature, sharpness and length. The segment is solved as a G1 Hermite interpolation (Bertolazzi-Frego), matching both boundary positions and both boundary directions exactly.

## Details

- Pure geometry: no speed, no time. Dynamics are layered on by the search.
- Implements `IPolynomialPath` as the degenerate cubic (A = B = 0, C = sharpness, D = start curvature), so it serialises through the (A, B, C, D, L) container store like any polynomial path.
- The vendor solver works in its own plane (X, Z) with tangents measured from +X; NCPF measures direction from +Y, so the angle channel shifts by −90° on the way out.

## Used by

`ClothoidPathBuilder` bakes these on the Bake side; `RearFirstPathBuilder` wraps the solved segment into a rear-first primitive; `FuncControlSetContainer` stores them by polynomial coefficients.
