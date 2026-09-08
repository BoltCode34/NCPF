# Config

A node of the discrete lattice used by the NCPF search: cell coordinates (X, Y) plus a heading channel index (Angle).

## Details

- Angle is an index into the heading channel lattice, not degrees. Arithmetic operators never wrap it modulo the channel count; folding the index back into range is the caller's job.
- The cell part supports plain integer arithmetic: sum, difference, negation, scaling and integer division.
- `Cell` packs (X, Y) into an `Int2` from Core.Foundation.
- Value semantics: equality and hashing compare all three channels.

## Used by

Grid lattices (`IGridMap3D` family), `CurvateMap3D` search nodes, and every search that runs over the baked ControlSet.
