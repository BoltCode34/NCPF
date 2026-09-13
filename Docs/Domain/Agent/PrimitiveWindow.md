# PrimitiveWindow

The next few curves of the agent's path, each with its length, direction of travel and speed limit. The travel planner plans speed over it.

`NCPF.Domain` · `PrimitiveWindow.cs`, `WindowPart.cs`

## Description

To choose a speed for the current curve, the agent has to know what comes after it: a sharp turn ahead means braking now. PrimitiveWindow collects the curves from the agent's position forward until at least `lookahead` metres are covered. The last curve is always included whole, never cut in the middle.

For each curve it records a `WindowPart`:

| Field | Meaning |
|---|---|
| `Length` | The length of the part, in metres. For the current curve, only what remains. |
| `Gear` | `+1` driving forward, `−1` in reverse. |
| `KappaPeak` | The sharpest curvature on the part, in radians per metre. |
| `VCap` | The highest speed the part allows: `min(vTop, √(aMax / κ), ωMax / κ)`. |

It is built once per replan and read by [SpacedMapPV](../../Pipeline/Application/Space/SpacedMapPV.md).

## Constructor

```csharp
PrimitiveWindow(PathAgentModel model, float lookahead, float vTop, float aMax, float wMaxDegPerSec)
```

| Parameter | Purpose |
|---|---|
| `model` | The agent, for its path and position. |
| `lookahead` | How many metres ahead to cover. At least 0.2. |
| `vTop` | The top speed. |
| `aMax` | The acceleration, limiting speed in turns. |
| `wMaxDegPerSec` | The turning rate, limiting speed in turns. |

## Key API

```csharp
float TotalArc { get; }
float[] Arc
float RemainingArc(int part)
```

The length of the whole window, the distance from the agent to the start of each part, and the distance from a part's start to the window's end.

```csharp
static float KappaAt(IPath path, float arc)
```

The curvature right at `arc`, measured over a short step. Unlike `KappaPeak`, this is the local value, used to limit speed at every moment.

## Notes

- Curvature here is measured by sampling the heading, not from the curve's coefficients, so it can differ slightly from [PrimitiveAnalyzer](PrimitiveAnalyzer.md), which the search uses.

## Related

- [SpacedMapPV](../../Pipeline/Application/Space/SpacedMapPV.md) — the speed graph over it.
- [TravelPlannerFeature](../../Pipeline/Presentation/TravelPlannerFeature.md) — builds it.
