# Projector3D

The pure projector: projects the primitive library, baked at the origin pose, onto the requested state — and onto it alone. A pure translation, no foreign layers, no analysis, no prices.

`NCPF.Pipeline.Application · Space/Projector3D.cs`

## What it is

The layered library ([[DiscretizedControlSet3D]]) stores the heading each primitive starts under, baked at the origin. Projecting onto `(x, y, θ)` takes **only layer θ**: the primitives already start under the right heading, so the projection is a translation by `(nodePos − origin)`. No in-place turn — the heading is part of the node, not changed by the projector.

Output is primitives ([[DiscretizedPath3D]]), not transitions: no neighbor id, no weight. Each projected shape is wrapped in a [[PathOffset]] (carrying the raw library entry as `Inner`) with its cell sweep offset to the node.

## Constructor

```csharp
Projector3D(DiscretizedControlSet3D controlSet)
```

`controlSet` — the primitive library, layered by start heading. Layer θ is what gets projected when expanding a node whose angle is θ.

## Key API

```csharp
DiscretizedPath3D[] GetPrimitives(Config startCell, IGridMap3D grid)
```
Projects the one matching layer onto the node: shifts every primitive of layer `startCell.Angle` from the origin to the node's world position, and offsets its cell sweep to `(startCell.X, startCell.Y)`.

**Args**
`startCell` — the node `(x, y, direction)` to expand
`grid` — the lattice: world↔cell for the origin/node translation

**Return** array of [[DiscretizedPath3D]] — the projected primitives (no weights)

## What is not here

- **No turn-in-place.** The heading changes by driving a curve, not by a stationary rotation. There is no turn cost here.
- **No pricing.** Weights are the [[CurvateEdgeGenerator]]'s duty.
- **No collision.** Whether the sweep fits is [[ControlSetValidator3D]]'s job.

## Related

- [[IPrimitivesProjector3D]] — the contract
- [[ControlSetValidator3D]] — collision filter that wraps this
- [[CurvateEdgeGenerator]] — the pricer that runs the chain
- [[PathOffset]] — the translation wrapper (carries `Inner`)
- [[DiscretizedControlSet3D]] — the library
- [[CurvateMap3D]] — the consuming graph