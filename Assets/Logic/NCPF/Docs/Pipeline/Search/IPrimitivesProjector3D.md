# IPrimitivesProjector3D

Projects the geometric primitive library onto a 3D search node `(x, y, direction)`. Purely geometric: no velocity loop, no speed buckets, no centripetal κ-cap.

`NCPF.Pipeline.Application · Space/IPrimitivesProjector3D.cs`

## Contract

```csharp
public interface IPrimitivesProjector3D
{
    DiscretizedPath3D[] GetPrimitives(Config startCell, IGridMap3D grid);
}
```

Output is **primitives**, not transitions — no neighbor id, no weight. Assembling or pricing transitions is the [[CurvateEdgeGenerator]]'s job.

The contract is decorator-friendly for **existence** capabilities (collision validation wraps this interface); **analysis** and **pricing** belong to the edge generator, never to a projector.

## Implementations

- **[[Projector3D]]** — the pure projection: a translation of the library layer onto the node.
- **[[ControlSetValidator3D]]** — decorator: keeps only primitives whose whole cell sweep fits the grid.

Usual assembly: `ControlSetValidator3D(Projector3D(...))`, then wrapped by [[CurvateEdgeGenerator]] for pricing.

## Related

- [[Projector3D]] — the primary implementation
- [[ControlSetValidator3D]] — collision filter
- [[CurvateEdgeGenerator]] — the pricer that runs this chain
- [[DiscretizedPath3D]] — what is returned
- [[Config]] — the node