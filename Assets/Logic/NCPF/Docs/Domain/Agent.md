# Agent

A movable body shape for map baking: position of its local origin on the world grid, heading and cell size, plus the boolean footprint the world is convolved with.

## Details

- The anchor defaults to the centre of the footprint's filled bounds; subclasses override `GetLocalAnchor` for custom tow points.
- `Rectangle` is the concrete shape: width/height in metres, an odd NxN footprint matrix sized from the diagonal (same for every rotation), and `Anchor` as a fraction of the rectangle — (0.5, 1) is a needle towed by its tip. The rotated anchor shares axes with the footprint so the two never drift apart.

## Used by

`MapBuilder` convolves the footprint; `AgentConfig` (a ScriptableObject port) hands the concrete agent to the bake at runtime.
