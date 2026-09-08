# RearFirstPath

The natively baked rear-first primitive: a wrapper over a polynomial path whose inner clothoid was G1-solved with both tangents +180°, so its endpoints land exactly on lattice nodes (a mirrored path would not, except on axes and diagonals).

## Details

- The inner path speaks motion tangents (heading + 180°); the wrapper takes the 180° back off, restoring the nose-heading channel the lattice stores. Without the shift a forward→rear-first junction would jump by 180°.
- Positions, length and curvature pass through untouched — a pure angle-channel shift.
- `IRearFirstPath` is the marker interface: the runtime gear detection (`PathGearUtility`) tells rear-first primitives apart by the marker, not by the concrete class.

## Used by

`RearFirstPathBuilder` produces these on the Bake side; `PathGearUtility` consumes the marker at runtime.
