# FacingModeMap

The graph [LookAtTargetFeature](../../../Pipeline/Presentation/LookAtTargetFeature.md) searches to decide, along the next few metres of the path, when to face the target, when to face along the path, and how fast to move.

`NCPF.Pipeline.Application` · `FacingModeMap.cs`

## Description

A node is a **sample** of the [MotionWindow](../Agent/MotionWindow.md), a **facing mode** and a **speed** value.

```csharp
public enum FacingMode { Path, Target }
```

The facing is not a free angle. In the `Path` mode it is the path's heading at that sample, the body orientation the path was planned with; in the `Target` mode it is the direction from that sample to the target. The first sample is where the body already is, so both of its modes stand for the body's current facing.

A node is free if the body fits at that sample with that facing. In the `Target` mode the speed must also be within the aim speed.

An edge goes from a sample to one of the next `maxSpan` samples, possibly changing mode and speed. It exists only if:

- the speed change fits the acceleration over that distance;
- the speed at the end is not too high for the curvature there;
- the facing change, including a switch of mode, fits the turn rate within the time the step takes;
- the body stays clear of obstacles while it turns and moves between the two samples.

Because a switch has to fit the turn rate, it is never instant: a large turn needs a slow step.

### Weights

An edge's weight is the time spent **not** facing the target: the step's full time if either end is in the `Path` mode, and almost nothing if both ends face the target. A step that ends the window in the `Path` mode also pays the time the turn back to the target would take.

Because facing the target is almost free, slowing down to keep facing it never makes a plan worse, and the remaining cost can be zero. The search therefore uses no estimate (`ZeroHeuristic`); the window is small enough for that.

## Constructor

```csharp
FacingModeMap(MotionWindow window, IGridMap3D map3D, float startAngle, int speedCount,
              float vMax, float aimSpeed, float wMaxDegPerSec, float aMax, int maxSpan)
```

| Parameter | Purpose |
|---|---|
| `window` | The samples of the path ahead. |
| `map3D` | Whether the body fits at a sample and facing. |
| `startAngle` | The body's current facing, in degrees. |
| `speedCount` | The number of speed values, zero included, evenly spaced up to `vMax`. At least `2`. |
| `vMax` | The highest speed value. |
| `aimSpeed` | The highest speed in the `Target` mode. Raised to at least the lowest non-zero speed value. |
| `wMaxDegPerSec` | The turn rate, in degrees per second. |
| `aMax` | The acceleration. |
| `maxSpan` | How many samples an edge may skip. |

## Key API

```csharp
int Id(int sample, FacingMode mode, int speed)
void Decode(int id, out int sample, out FacingMode mode, out int speed)
float SpeedOf(int speed)
int SpeedToIndex(float speed)
float Facing(int sample, FacingMode mode)
```

Converts between nodes and ids, between speed indices and metres per second, and returns the facing a node stands for.

## Related

- [LookAtTargetFeature](../../../Pipeline/Presentation/LookAtTargetFeature.md) — builds and searches it.
- [MotionWindow](../Agent/MotionWindow.md) — what it is built over.
- [SpacedMapPV](SpacedMapPV.md) — the speed-only graph of the standard main planner.
