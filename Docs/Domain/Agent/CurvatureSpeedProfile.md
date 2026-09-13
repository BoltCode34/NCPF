# CurvatureSpeedProfile

The highest safe speed at every point of a curve, taking both its turns and the need to brake before them into account.

`NCPF.Domain` · `CurvatureSpeedProfile.cs`

## Description

Planning gives each curve a target speed at its end. Between the ends, the agent still has to respect every turn along the way and start braking early enough. CurvatureSpeedProfile computes that limit.

It works in two steps:

1. **Turn limits.** The curve is sampled every few centimetres. At each sample, the limit is `min(vTop, √(aMax / κ), ωMax / κ)`: slow enough to hold the turn with the available sideways acceleration and turning rate.
2. **Braking.** Going backward from the end, where the limit is the planned exit speed, each limit is lowered so the agent can still slow down in time for the next: `v ≤ √(vNext² + 2 · aMax · step)`.

The result: on gentle stretches the limit is the top speed; before a sharp turn or a slow exit it drops, starting exactly one braking distance ahead.

Used by [TravelPlannerFeature](../../Pipeline/Presentation/TravelPlannerFeature.md).

## Key API

```csharp
static CurvatureSpeedProfile Build(IPath part, float startArc, float vTop, float aMax,
                                   float wMaxDegPerSec, float exitSpeed)
```

| Parameter | Purpose |
|---|---|
| `part` | The curve. |
| `startArc` | Where on the curve to start, in metres. |
| `vTop` | The top speed. |
| `aMax` | The acceleration. |
| `wMaxDegPerSec` | The turning rate, in degrees per second. |
| `exitSpeed` | The speed to reach at the curve's end. Its sign is ignored. |

```csharp
float Cap(float arc)
```

The speed limit at `arc` metres along the curve, interpolated between samples. Before `startArc`, the first value; past the end, `exitSpeed`. Always positive.

## Related

- [PrimitiveWindow](PrimitiveWindow.md) — speed limits per whole curve.
- [DynamicAgent](../Core/DynamicAgent.md) — the limits.
