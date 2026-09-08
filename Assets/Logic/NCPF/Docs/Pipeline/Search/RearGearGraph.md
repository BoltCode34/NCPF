# RearGearGraph

A graph decorator — the mission-tier gear preference: rear-first transitions get their weight multiplied by `gearPenalty` (1 = neutral). To the model reversing is exactly as legal and as fast as going forward; to the mission it is exceptional (the eyes face forward).

`NCPF.Pipeline.Application · Space/RearGearGraph.cs`

## What it is

A decorator over [[ICurvateGraph]]. It does not add or remove edges — it re-prices the rear-first ones. Casual reversing becomes expensive; the exceptional case (where rear is the only/best route) still wins because the multiplier only raises the price, it does not forbid.

Rear-first is recognized by the [[IRearFirstPath]] marker beneath the wrappers ([[PathGearUtility]].`IsRearFirst`), not by geometry sniffing.

## Constructor

```csharp
RearGearGraph(ICurvateGraph decoratee, float gearPenalty)
```

`decoratee` — the graph whose rear-first edges are re-weighted (usually [[CurvateMap3D]]).

`gearPenalty` — the multiplier for rear-first edges. Clamped to ≥ 1 (1 = neutral; higher = reversing more expensive).

## Key API

```csharp
CurvateTransition[] GetNeightbors(int id)
```
Takes the decoratee's edges and multiplies the weight of every rear-first one by `gearPenalty`. The edge set is unchanged.

`bool PointFree(int id)` — forwarded unchanged.

## Details

- The curvature and base-time knobs live in [[CurvateEdgeGenerator]]; the gear knob is a separate decorator so the two preferences compose without coupling.
- Both the planner and the rope agent ([[ChainCurvateAgent]]) can search the same decorated graph — the graph knows nothing of its consumers.

## Related

- [[ICurvateGraph]] — the contract it decorates
- [[CurvateMap3D]] — the usual decoratee
- [[CurvateEdgeGenerator]] — the base-time and curvature knobs
- [[PathGearUtility]] — `IsRearFirst`, the recognition
- [[IRearFirstPath]] — the marker beneath the wrappers