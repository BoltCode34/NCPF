# BackwardLayerBuilder

The layer-doubling decorator: the decoratee's forward primitives plus an own rear-first run, stored in the layer interleaved.

## Details

- The rear-first pass (`RearFirstLayerBuilder`) runs the base engine "inside-out": every primitive is solved by `RearFirstPathBuilder` — G1 with both tangents +180° — so ends land exactly on lattice nodes at EVERY layer (a 180° rotation is a lattice symmetry always, unlike an axis reflection which only holds on 45° layers).
- The runs are independent on purpose: the same node pair legitimately owns two curves, nose-first and rear-first; cross-decomposition of a rear-first primitive through a forward node is deliberately out of the first cut.

## Used by

`ControlSetBakeService` wires it as the layer engine of the `ControlSetBuilder`.
