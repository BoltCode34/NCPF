# ControlSetBaker

The bake root on the sandbox scene: reads the map through the port, runs the bake service and writes the control set back through the port.

## Details

- ContextMenu "Bake" bakes the stored quarter; "Bake Layer" a single heading layer (`_angleLayer`).
- Handles: max radius, minimum turn radius (0 = disabled), limiter tube scale, and the length multiplier (default 1.4) of the equivalence check.
- Gizmos draw the baked layer's primitives when `_draw` is on, from the in-memory set right after a bake or from the asset otherwise.

## Used by

The NCPFTest sandbox scene; nothing references it in game code.
