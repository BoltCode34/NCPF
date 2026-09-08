# FuncControlSetContainer

The concrete <see cref="ControlSetAsset"/>: stores the baked control set by cubic curvature coefficients — each primitive is (A,B,C,D,L) plus its end node.

## Details

- A clothoid is the degenerate cubic (A=B=0) and serialises through the same store; on read it is rebuilt as the exact Euler-spiral segment the G1 solver produced, bit-identical to the bake. A genuine cubic spiral has no closed form and is integrated finely (step ∝ length).
- Only a quarter of the lattice is stored; the other three quadrants are rebuilt natively on read — same curvature function, rotated boundary frame (`QuarterSymmetry`).
- Rear-first entries are baked natively with heading+180 tangents; the `RearFirstPath` wrapper takes the 180° back off into the nose-heading channel.
- Pure geometry in and out; a Json fallback file sits beside the asset (hard-coded path is a phase-2 concern).

## Used by

Held through the `ControlSetAsset` port by `PathPlanner` and `ControlSetBaker`; no assembly references the concrete type.
