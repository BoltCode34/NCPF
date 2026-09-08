# NCPFMapBuilder

Bakes the C-space map from the 2D occupancy grid and writes it into the map asset through the port.

## Details

- Inverts the grid's free/blocked cells into an occupancy field, convolves it with the agent footprint (`MapBuilder`) over the configured number of heading layers, and stores the `Map3D` through `MapAsset`.
- Optional rebake on Awake in the DI scene stays as is (a phase-2 concern).

## Used by

The DI scene wires it next to the `Unity2DGrid`; the search reads the result through the map asset.
