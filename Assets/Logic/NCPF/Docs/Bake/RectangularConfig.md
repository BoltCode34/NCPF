# RectangularConfig

Rectangular agent footprint: width and length in world units, orientation, and the anchor point the body is dragged by.

## Details

- The anchor lives in rectangle fractions — (0,0) bottom-left, (1,1) top-right — and must match the point the body is dragged by in game; the map must be rebaked after changing it.
- `FootprintSize`/`FootprintAnchor` feed both verticals' drawing; `GetAgent` yields the discrete `Rectangle` the bake convolves the world with.

## Used by

`NCPFMapBuilder` (map bake), `PathPlanner` and `DynamicsVisualizer` (footprint drawing) — all through the `AgentConfig` port.
