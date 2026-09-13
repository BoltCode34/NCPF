# PathAgentModel

What an agent is at this moment: the path it follows, how far along it is, and its current state. It holds this data and decides nothing.

`NCPF.Domain` · `PathAgentModel.cs`

## Description

PathAgentModel is the single source of truth about a driving agent. Everything that decides something, such as how fast to go or where to look, is done by the passes in [PathFollowAgent](../../Pipeline/Application/Agent/PathFollowAgent.md), which then write the result back here.

The position along the path is a **distance**, not a time: the path is geometry only, so the model tracks which part the agent is on and how many metres into it.

### Replacing the path

There are two ways to give the model a new path:

- **`StartFollowing`** — a fresh start. The position resets to the start of the new path. The facing is kept, and the speed is kept only if the first curve is driven in the same direction; otherwise it drops to zero.
- **`ContinueWithStateSaving`** — a seamless swap while driving. The new path must begin with the curve the agent is on now, which [PathFindingAgent.StitchPath](../../Pipeline/Application/Search/PathFindingAgent.md) guarantees. The distance into that curve and the whole state are kept, so the agent neither stops nor jumps.

## Key API

```csharp
void StartFollowing(IPath[] path, DynamicState startPos)
void ContinueWithStateSaving(IPath[] path)
```

Starts a new path, or swaps in a stitched one. An empty path passed to `ContinueWithStateSaving` is ignored; to `StartFollowing`, it clears the path and stops the agent.

```csharp
WorldConfig AdvanceByArc(float ds)
```

Moves `ds` metres along the path, across parts as needed, stopping at the end. Returns the path's pose there. It does not change the state.

```csharp
void Apply(DynamicState state)
```

Writes the planned state.

```csharp
List<WorldConfig> PeekAheadArc(float dsStep, float maxMeters, int maxSamples = 128)
```

Returns poses ahead of the current position, every `dsStep` metres, up to `maxMeters`, without moving.

| Property | Meaning |
|---|---|
| `CurrentDynamicState` | The last state written. Speed is negative while driving a curve in reverse. |
| `Parts`, `CurrentPart`, `CurrentPartIndex` | The path and the part the agent is on. |
| `CurrentPartArc` | Metres into the current part. |
| `HasPath` | Whether there is a path. |
| `Completed` | Whether the end of the path has been reached. |

## Related

- [PathFollowAgent](../../Pipeline/Application/Agent/PathFollowAgent.md) — drives the model each frame.
- [PathAgentPipeline](../../Pipeline/Application/Agent/PathAgentPipeline.md) — gives it new paths.
- [DynamicState](../Core/DynamicState.md) — the state it holds.
