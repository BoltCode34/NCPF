# ITransitionGenerator

Produces the priced edges leaving one lattice pose.

`NCPF.Pipeline.Application` · `ITransitionGenerator.cs`

## Description

```csharp
public interface ITransitionGenerator
{
    TransitionData[] GetPrimitives(Config startCell, IGridMap3D grid);
}
```

Given a pose, a generator returns every curve that can be driven from it, as edges with a destination and a weight. How it decides that a curve fits, and how it prices it, is up to the implementation.

Weights are part of the output because decorators over the graph ([RearGearGraph](RearGearGraph.md) and your own) adjust them.

Implement this interface to change how edges are produced without touching the rest of the search, for example to add terrain cost. Return a **new** array on every call: decorators above you modify the returned edges in place.

## Related

- [PassabilityTransitionGenerator](PassabilityTransitionGenerator.md) — the standard implementation.
- [CurvateMap3D](CurvateMap3D.md) — the graph that uses it.
