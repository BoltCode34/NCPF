# IPrimitivesProjector3D

The older way of producing edges: a chain that places curves at a node, filters out those that collide, and prices the rest. It is kept in the package but is **not used** by the standard setup.

`NCPF.Pipeline.Application` · `IPrimitivesProjector3D.cs`, `Projector3D.cs`, `ControlSetValidator3D.cs`, `PassabilityValidator3D.cs`, `CurvateEdgeGenerator.cs`

## Description

```csharp
public interface IPrimitivesProjector3D
{
    DiscretizedPath3D[] GetPrimitives(Config startCell, IGridMap3D grid);
}
```

A projector returns the curves available at a pose, placed at that pose. Decorators filter them. The chain was:

| Type | Role |
|---|---|
| `Projector3D` | Takes the control set's curves for the pose's heading and moves them to the pose. |
| `ControlSetValidator3D` | Keeps only curves whose every cell is free. Checks them on every call. |
| `PassabilityValidator3D` | The same filter, read from a baked [PrimitivePassability](../../../Domain/Space/PrimitivePassability.md) mask. |
| `CurvateEdgeGenerator` | Runs the chain and prices the survivors into [CurvateTransition](ICurvateGraph.md) edges. |

The chain builds a full moved copy of every curve at every expanded node, and most copies are thrown away by the next filter. [PassabilityTransitionGenerator](PassabilityTransitionGenerator.md) replaced it and produces the same edges from tables.

`CurvateEdgeGenerator` no longer implements [ITransitionGenerator](ITransitionGenerator.md), so the chain cannot be plugged into [CurvateMap3D](CurvateMap3D.md) as is. It remains as a readable reference of the pricing.

## Notes

- `PassabilityValidator3D` identifies curves by their **position** in the list it receives. It must wrap a `Projector3D` directly: any filter between them shifts positions, and the mask then answers about the wrong curve.

## Related

- [PassabilityTransitionGenerator](PassabilityTransitionGenerator.md) — the replacement.
