# ClothoidPathBuilder

Builds a `ClothoidPath` between two poses by solving the G1 Hermite interpolation problem (Bertolazzi–Frego).

## Details

- Pure geometry — the bake-side builder; dynamics live in the Pipeline assembly.
- Convention bridge: NCPF measures direction from +Y (CCW), the clothoid solver from +X (CCW), so angles get +90° on the way in — and `ClothoidPath` takes it back off on the way out.
- `RearFirstPathBuilder` is its rear-first twin: the same G1 problem with both tangents flipped by 180°, wrapped in `RearFirstPath`.

## Used by

The layer bake via `IPathBuilder`; `ControlSetBakeService` puts it at the bottom of the chain.
