# MapBuilder

Bakes a `Map3D` from a 2D obstacle world: for every direction layer the agent footprint is convolved over the grid, then the resulting rig snapshot is packed with the map.

## Details

- `Map` is the input world: a boolean obstacle grid plus cell size.
- The agent is rotated per layer (its `Angle` is mutated during the bake) and reset to zero afterwards; the rig is captured from the zero-degree footprint.
- Convolution is `ConvolutionUtility.Convolve`: a cell is free only where the anchored kernel fits without touching an obstacle.

## Used by

`NCPFMapBuilder` drives it from scene data on the Bake side.
