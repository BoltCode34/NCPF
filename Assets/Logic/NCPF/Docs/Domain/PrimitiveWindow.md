# PrimitiveWindow

The speed window: the chain of primitives lookahead metres ahead of the cursor, ROUNDED UP to a primitive boundary — the horizon decides whether to start the next primitive, but never cuts the one it started.

## Details

- Built once per replan and read by the map and the heuristic; geometry is not re-asked from the model (the 3D search already validated it) — only length, gear and curvature matter here.
- Each `WindowPart` carries the primitive's full remainder, its gear, the peak tangent curvature and the speed ceiling from all three DynamicAgent limits.
- `KappaAt` probes the LOCAL curvature at an arc (forward probe, backward near the end) — the runtime speed clamps price their limits from it at every instant.

## Used by

The PV ladder and the travel heuristic read it; `CurvatureSpeedProfile` shares the same probe style along a single primitive.
