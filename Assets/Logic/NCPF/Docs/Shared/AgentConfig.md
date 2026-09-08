# AgentConfig

The abstract ScriptableObject port of the agent footprint: yields the collision `Agent` the bake convolves the world with, at a given grid resolution.

## Details

- One asset answers "what shape is the body" for both the map bake and the control-set bake, keeping the two consistent.
- Concrete configs decide the footprint shape; `RectangularConfig` describes a rotated rectangle with an anchor point.

## Used by

`NCPFMapBuilder` feeds it into the `MapBuilder`; `PathPlanner` draws the agent footprint with it.
