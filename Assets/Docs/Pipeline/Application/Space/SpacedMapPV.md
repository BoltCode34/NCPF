# SpacedMapPV

The graph the travel planner searches to choose a speed for each curve of the path ahead.

`NCPF.Pipeline.Application` · `SpacedMapPV.cs`, `ConfigPV.cs`

## Description

Once the path is chosen, the one thing still open is how fast to drive each part of it. SpacedMapPV turns that into a small search.

A node is a `ConfigPV`: **which part** of the [PrimitiveWindow](../../../Domain/Agent/PrimitiveWindow.md) the agent is entering, and **at what speed**. Speeds are whole steps of `vTop / speedSteps`, signed: positive for driving forward, negative for reversing. An edge goes from the start of a part to the start of the next part at a new speed. Its weight is the time the part takes at the average of the two speeds.

An edge exists only if:

- its speed has the part's direction, or is zero;
- its end speed does not exceed the speed limit of this part or the next;
- the speed change fits the agent's acceleration over the part's length.

Because a forward speed and a reversing speed only meet at zero, every change of direction passes through a full stop, as it would for a real body.

There are no obstacles in this graph: the path was already checked against them.

`PrimitiveEndAchivement` counts arrival at the end of the window; `PrimitiveTimeHeuristic` estimates the remaining time at top speed.

## Constructor

```csharp
SpacedMapPV(PrimitiveWindow window, float vTop, float aMax, int speedSteps, float speedStrive = 0f)
```

| Parameter | Purpose |
|---|---|
| `window` | The parts of the path ahead, with their lengths, directions and speed limits. |
| `vTop` | The highest speed step. |
| `aMax` | The agent's acceleration, limiting speed changes. |
| `speedSteps` | The number of speed steps in each direction. The graph has `2 · speedSteps + 1` speeds per part. |
| `speedStrive` | An extra cost per metre for driving below `vTop`, pushing the plan toward high speeds. `0` disables it. |

## Related

- [TravelPlannerFeature](../../../Pipeline/Presentation/TravelPlannerFeature.md) — builds and searches it.
- [PrimitiveWindow](../../../Domain/Agent/PrimitiveWindow.md) — what it is built over.
- [FacingModeMap](FacingModeMap.md) — the facing-and-speed graph of the look-at-target planner.
