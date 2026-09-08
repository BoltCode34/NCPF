# ITransitionGenerator

The graph-facing abstraction: a cell in — weighted transitions out. It is the [[CurvateMap3D]] graph's delegate for the part of the graph's duty that concerns weights.

`NCPF.Pipeline.Application · Space/ITransitionGenerator.cs`

## Contract

```csharp
public interface ITransitionGenerator
{
    CurvateTransition[] GetPrimitives(Config startCell, IGridMap3D grid);
}
```

Primitives are abstracted away from weight: the implementation runs the primitive set through the projector chain (projection, validation — existence) and prices the survivors. The weight nonetheless remains the **graph's** duty — its decorators rely on exactly the outgoing transition weights — so the generator is the graph's delegate for weight handling, not a free-standing pricer.

## Implementation

- **[[CurvateEdgeGenerator]]** — the single owner of a transition's price: runs the projector chain and prices survivors in one formula (honest time + the mission tightness tax).

## Related

- [[CurvateMap3D]] — the graph that delegates to it
- [[CurvateEdgeGenerator]] — implementation
- [[IPrimitivesProjector3D]] — the existence chain it runs
- [[CurvateTransition]] — what it returns