# ControlSetValidator3D

Collision decorator: keeps only primitives whose whole cell sweep fits the grid. It decorates [[IPrimitivesProjector3D]] — takes the primitives the inner projector yields and drops any whose shape is blocked anywhere.

`NCPF.Pipeline.Application · Space/ControlSetValidator3D.cs`

## Constructor

```csharp
ControlSetValidator3D(IPrimitivesProjector3D decoratee)
```

`decoratee` — the projector whose primitives are filtered (usually [[Projector3D]]). The validator generates no primitives of its own — it only removes blocked ones.

## Key API

```csharp
DiscretizedPath3D[] GetPrimitives(Config startCell, IGridMap3D grid)
```
Takes the primitives from the decoratee and keeps only those whose whole sweep is free.

**Args**
`startCell` — the node to expand from
`grid` — the lattice for free-cell queries

**Return** the same [[DiscretizedPath3D]] set, minus the blocked ones

## What it does

`CheckPath` walks the shape's `IdSequence` ([[DiscretizedPath3D]]) and requires **every** cell to be free (`grid.Cell3DFree`). One occupied cell drops the whole primitive.

This is the **drive** collision — whether the body fits along the primitive. The **turn** collision (heading change) is no longer a separate step: the heading changes only by driving a curve, so the swept cells already cover it.

## Usual assembly

```csharp
new ControlSetValidator3D(new Projector3D(discretized))
```
then wrapped by [[CurvateEdgeGenerator]] for pricing. The projector yields all geometrically possible primitives; the validator drops the ones that meet a wall.

## Related

- [[IPrimitivesProjector3D]] — the contract it decorates
- [[Projector3D]] — the usual decoratee
- [[DiscretizedPath3D]] — whose `IdSequence` it reads
- [[CurvateEdgeGenerator]] — the pricer that runs the chain
- [[CurvateMap3D]] — the graph where this lands