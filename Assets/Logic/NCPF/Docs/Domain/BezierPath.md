# BezierPath

A cubic Bezier curve between two poses: start and end positions come from the boundary poses, the two middle control points are free. Kept as the one general-purpose non-polynomial shape in the library.

## Details

- Length is approximated by chord sampling; `Evaluate` maps arc length to the curve parameter through that approximation.
- Heading is derived from the curve tangent, in the NCPF convention (0 = +Y, CCW).

## Used by

Sandbox and tooling paths; not part of the baked control set (clothoids and cubics are).
