# ControlSetBuilder

Bakes a geometric control set (Pivortaiko/Kelly §5.1): node = `Config` (cell x, y, direction of travel), path = `IPath`, grid = `IGridGeometry3D`.

## Details

- The bake knows nothing about dynamics — speed is layered on by the search. Only the first quadrant [0..90°) of start directions is baked; the remaining quadrants are reconstructed natively on read (`QuarterSymmetry`).
- The layer engine (`LayerControlSetBuilderBase`) grows concentric rings of target cells, sweeps end headings ±2 steps around the chord bearing, and prunes through an `IPathLimiter` — divisibility is re-checked after the whole ring registers, so same-ring candidates decompose through each other.
- maxRadius is a safety cap only; the real stop is ring decomposition.

## Used by

`ControlSetBakeService` assembles the chain; `RearFirstLayerBuilder`/`BackwardLayerBuilder` extend the layer engine with the rear-first pass.
