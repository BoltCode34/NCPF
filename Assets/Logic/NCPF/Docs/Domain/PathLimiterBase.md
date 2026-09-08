# PathLimiterBase

The geometric admission filter of the layer bake: length ratio, turn radius and path decomposition.

## Details

- The equivalence tube derives from the GRID resolution (half a cell, half an angle step by default) instead of hand-tuned absolute limits.
- `CheckLength` bounds the length-to-chord ratio (breakDistance, default 1.4); `CheckTurnRadius` rejects turns sharper than the agent's minimum radius (0 = disabled); `CanBeDivided` drops a primitive whose sample passes through another node's tube — a shorter primitive already reaches that node.
- No kinematic check: feasibility w.r.t. speed is decided at search time.

## Used by

`LayerControlSetBuilderBase` prunes its candidates through `IPathLimiter`; the bake chain wires the defaults.
