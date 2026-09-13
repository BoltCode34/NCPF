# BackwardLayerBuilder

Adds reversing curves to a heading: it runs the forward bake, then a separate tail-first bake, and stores both in the same layer.

`NCPF.Bake.Application` · `BackwardLayerBuilder.cs`

## Description

An agent that can reverse needs curves it drives tail-first. Mirroring forward curves does not work: a mirrored curve lands exactly on lattice nodes only for some headings. So BackwardLayerBuilder bakes reversing curves natively, with a `RearFirstLayerBuilder` and a path builder that solves each curve with both ends turned 180°.

The forward and reversing bakes are independent. Each keeps its own rings and its own record of kept nodes, so the same pair of nodes can own two curves: one driven nose-first and one driven tail-first. The layer returned is the forward curves followed by the reversing ones.

It is a decorator over another `ILayerControlSetBuilder`, which provides the forward curves.

## Constructor

```csharp
BackwardLayerBuilder(ILayerControlSetBuilder decoratee, IPathLimiter limiter, IPathBuilder rearPathBuilder)
```

| Parameter | Purpose |
|---|---|
| `decoratee` | Bakes the forward curves. |
| `limiter` | Decides which reversing curves are kept. |
| `rearPathBuilder` | Produces reversing curves, usually a `RearFirstPathBuilder` wrapping the forward builder. See [IPathBuilder](IPathBuilder.md). |

## Key API

```csharp
IPath[] BuildLayer(IGridGeometry3D gridSpace, int maxCell, WorldConfig origin)
```

Returns the forward curves of the heading followed by its reversing curves.

## Related

- [LayerControlSetBuilderBase](LayerControlSetBuilderBase.md) — the bake it runs twice.
- [RearFirstPath](../../Domain/Paths/RearFirstPath.md) — how a reversing curve is represented.
- [RearGearGraph](../../Pipeline/Application/Space/RearGearGraph.md) — makes reversing more expensive at runtime.
