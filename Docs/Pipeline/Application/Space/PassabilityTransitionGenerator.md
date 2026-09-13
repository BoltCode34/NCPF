# PassabilityTransitionGenerator

The standard transition generator. It produces a node's edges from precomputed tables, with no geometry work during the search.

`NCPF.Pipeline.Application` · `PassabilityTransitionGenerator.cs`

## Description

Everything this generator needs is prepared up front, so expanding a node during the search is only table lookups:

- **Which curves fit** comes from a [PrimitivePassability](../../../Domain/Space/PrimitivePassability.md) mask baked by [PassabilityBaker](PassabilityBaker.md).
- **Where each curve ends** and **what it costs** are computed once per curve in the constructor. Both depend only on the curve itself, not on where it is placed, which is why they can be computed in advance.

For a node, the generator walks the control set's curves for that node's heading, skips those the mask rejects and those of zero length, and returns an edge for each of the rest.

### Edge weight

The weight estimates how long the curve takes to drive, plus an optional penalty for tight turns:

```
weight = baseMultiplier · length / cap  +  curvatureMultiplier · κ² · length
cap    = min(vMax, √(aMax / κ), ωMax / κ)
```

`κ` is the curve's peak curvature. `cap` is the highest speed the agent could hold through that curvature, limited by top speed, sideways acceleration and turning rate. A straight curve has no curvature, so its cap is the top speed and it pays no turn penalty.

## Constructor

```csharp
PassabilityTransitionGenerator(DiscretizedControlSet3D controlSet, PrimitivePassability passability,
                               DynamicAgent agent, float curvatureMultiplier = 0f, float baseMultiplier = 1f)
```

| Parameter | Purpose |
|---|---|
| `controlSet` | The curves, with their ids. |
| `passability` | Which curves fit at which node. Must be baked from the same control set and map. |
| `agent` | Top speed, acceleration and turning rate for the speed cap. |
| `curvatureMultiplier` | Weight of the tight-turn penalty. `0` disables it; around `0.1` makes a tight arc lose to a gentle one of the same turn. |
| `baseMultiplier` | Weight of the travel-time term. Keep it at `1`: below `1` the heuristics start to overestimate. |

## Key API

```csharp
TransitionData[] GetPrimitives(Config startCell, IGridMap3D grid)
```

Returns a new array of edges leaving `startCell`.

## Related

- [PassabilityBaker](PassabilityBaker.md) — bakes the mask.
- [PrimitiveAnalyzer](../../../Domain/Agent/PrimitiveAnalyzer.md) — measures peak curvature.
- [TransitionData](../../../Domain/Space/TransitionData.md) — the edges it returns.
- [IPrimitivesProjector3D](IPrimitivesProjector3D.md) — the older, slower chain it replaces.
