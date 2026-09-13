# SpacedMapAV

The graph the motion planner searches to choose where the agent faces and how fast it moves along a short stretch of path.

`NCPF.Pipeline.Application` · `SpacedMapAV.cs`, `ConfigAV.cs`, `IGridMapAV.cs`

## Description

Facing and speed cannot be planned separately: how far the body can turn over a stretch depends on how long it spends there. Slowing down buys time to turn. SpacedMapAV plans both together.

A node is a `ConfigAV`: a **sample** along the [MotionWindow](../Agent/MotionWindow.md), a **facing** step (one of the map's headings) and a **speed** step from `0` to `vMax`. A node is free if the body fits at that sample with that facing.

An edge goes from a sample to one of the next `maxSpan` samples, at a new facing and speed. Its weight is the time taken at the average speed. An edge exists only if:

- the speed change fits the agent's acceleration over that distance;
- the end speed is not too high for the curvature at the end sample;
- the facing change fits the turning rate within the time taken;
- the body stays clear of obstacles while it turns and moves between the two samples.

The last check matters: without it, a wide turn between two free samples could pass straight through a wall.

The graph states only what is physically possible. Preferences, such as looking at a target, are added by decorating its weights (see `MotionRulesGraph` in [MotionWindow](../Agent/MotionWindow.md)).

`IGridMapAV` is its contract, the facing-and-speed counterpart of [IGridMap3D](../../../Domain/Space/IGridMap3D.md). `WorldConfigAV` is a node in world units: degrees and metres per second.

## Constructor

```csharp
SpacedMapAV(MotionWindow window, IGridMap3D map3D, int velocityCount, float vMax,
            float wMaxDegPerSec, float aMax, int maxSpan)
```

| Parameter | Purpose |
|---|---|
| `window` | The samples of the path ahead. |
| `map3D` | Whether the body fits at a sample and facing. Its heading count sets the facing steps. |
| `velocityCount` | The number of speed steps, including zero. At least `2`. |
| `vMax` | The highest speed step. |
| `wMaxDegPerSec` | The turning rate, in degrees per second. |
| `aMax` | The acceleration. |
| `maxSpan` | How many samples an edge may skip. Larger values let the plan move faster in fewer steps, at the cost of more edges per node. |

## Key API

```csharp
int NearestFreeAngle(int sample, int desired)
```

Returns the facing step closest to `desired` in which the body fits at `sample`, or `desired` if none fits.

## Related

- [MotionPlannerFeature](../../../Pipeline/Presentation/MotionPlannerFeature.md) — builds and searches it.
- [MotionWindow](../Agent/MotionWindow.md) — what it is built over.
- [SpacedMapPV](SpacedMapPV.md) — the speed-only counterpart.
