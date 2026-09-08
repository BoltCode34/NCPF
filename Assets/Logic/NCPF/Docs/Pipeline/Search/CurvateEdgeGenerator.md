# CurvateEdgeGenerator

The edge generator: the single owner of a transition's price. Runs the primitive set through the projector chain (projection + validation) and prices the survivors in one formula — honest time plus the mission tightness tax. There is no in-place turn: the heading changes only by driving a curve.

`NCPF.Pipeline.Application · Space/CurvateEdgeGenerator.cs`

## Constructor

```csharp
CurvateEdgeGenerator(IPrimitivesProjector3D projector,
                     DiscretizedControlSet3D controlSet,
                     DynamicAgent agent,
                     float curvatureMultiplier = 0f,
                     float baseMultiplier = 1f)
```

`projector` — the existence chain (usually `ControlSetValidator3D(Projector3D(...)`): by a cell it yields the primitives that fit. The generator neither projects nor validates itself — it prices what survives.

`controlSet` — the primitive library, layered by start heading. Read once, eagerly in the constructor, to analyze each entry's curvature peak (`PrimitiveAnalyzer`). Analysis is taken over the library, not per node.

`agent` — the dynamic limits ([[DynamicAgent]]): `MaxVelocity`, `MaxLinearAcceleration`, `MaxAngledVelocity`. Three caps feed the speed ceiling the base time is divided by.

`curvatureMultiplier` — the mission tightness tax `κ_peak²·L` coefficient (0 = neutral; ~0.1 makes a tight arc lose to a gentle one at equal turn).

`baseMultiplier` — the share of honest time `L/cap` (1 = honest; below 1 breaks the positional heuristic's admissibility).

## Key API

```csharp
CurvateTransition[] GetPrimitives(Config startCell, IGridMap3D grid)
```
Yields the weighted transitions out of a node. Runs the projector, skips near-zero survivors, and prices each by [[#TravelWeight]]. A survivor is recognized by its library **reference** ([[PathOffset]].`Inner`) — so the chain may filter freely while analysis stays cached.

**Args**
`startCell` — the node `(x, y, direction)` to expand
`grid` — the lattice: cell↔world and free-cell queries

**Return** array of [[CurvateTransition]] (neighbor + shape + weight)

## TravelWeight

Two-term price: `baseMultiplier·L/cap + curvatureMultiplier·κ_peak²·L`.

The base is honest time — the `cap` cuts the speed ceiling at search time with the same three limits as the PV ladder: `v ≤ vMax`, `v ≤ √(a/κ_peak)`, `v ≤ w_max/κ_peak`. The second term is the mission tightness tax: at an equal turn angle a tight arc pays in proportion to κ; straights pay nothing.

## Details

- Thread safety by contract, not locks: analysis is immutable after the constructor. A transient generator is one instance, one client, one thread; shared access means a new class/decorator.
- The gear knob lives in [[RearGearGraph]], not here.

## Related

- [[ITransitionGenerator]] — the contract
- [[CurvateMap3D]] — the graph that owns it
- [[IPrimitivesProjector3D]] · [[Projector3D]] · [[ControlSetValidator3D]] — the existence chain
- [[PathOffset]] — library reference recognition (`Inner`)
- [[PrimitiveAnalyzer]] — the curvature analysis cached in the constructor
- [[RearGearGraph]] — the gear knob, a separate decorator