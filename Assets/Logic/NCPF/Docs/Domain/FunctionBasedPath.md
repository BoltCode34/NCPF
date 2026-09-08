# FunctionBasedPath

A path integrated from a curvature function κ(s): heading is the integral of curvature, position is the integral of the heading direction. The end pose is pinned to the supplied boundary at s = Length so the shape lands exactly on a lattice node.

## Details

- `FunctionBasedPath<T>` works with any `ICurvatureFunction`; `PolynomialBasedPath` is the cubic specialisation (κ(s) = A·s³ + B·s² + C·s + D) that also exposes `IPolynomialPath` for (A, B, C, D, L) serialisation. A clothoid is the degenerate cubic with A = B = 0, so one store serves both.
- `CubicPolyniomFunction` is the concrete cubic with an optional absolute-curvature clamp.
- Integration uses a coarse sample list plus local refinement per query, so `Evaluate` stays cheap without re-integrating the whole span.

## Used by

`FuncControlSetContainer` rebuilds polynomial paths from stored coefficients on read.
