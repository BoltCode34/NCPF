# PathFollowAgent

Drives an agent along a planned path. Each step, it asks the passes what to do, then moves the agent once.

`NCPF.Pipeline.Application` · `PathFollowAgent.cs`

## Description

A plan is pure geometry: it says where the path goes, not how fast to drive it or where to look. Those decisions come from **passes** ([SAPass](SAPass.md)).

Each call to `GetNext`:

1. reads the agent's current state from the [PathAgentModel](../../../Domain/Agent/PathAgentModel.md);
2. asks the **main pass** for a target speed and facing;
3. passes that decision through each **optional pass** in order, each free to change it;
4. advances the agent along the path by the final speed multiplied by the time step;
5. writes the new state back to the model.

Moving happens only once, after every pass has spoken, so the last pass has the final word and no pass can move the agent behind the others' backs.

## Constructor

```csharp
PathFollowAgent(PathAgentModel model, MainSAPass mainPlanner, params SAPass[] passes)
```

| Parameter | Purpose |
|---|---|
| `model` | Holds the path, the position along it, and the state. |
| `mainPlanner` | Decides speed and facing from the state. |
| `passes` | Optional passes applied after the main one, in order. |

## Key API

```csharp
DynamicState GetNext(float dt)
```

Runs one step and returns the agent's new state.

| Parameter | Purpose |
|---|---|
| `dt` | The time step, in seconds. |

```csharp
void StartFollowing(CurvateTransition[] geometry, DynamicState state)
```

Starts a new path from scratch, seeded with `state`: the agent's position, facing and speed.

```csharp
void ContinueWithStateSaving(CurvateTransition[] geometry)
```

Replaces the path while the agent keeps its position along it, its speed and its facing. The new path must begin with the curve the agent is currently on.

```csharp
List<SAPass> Passes { get; }
```

The optional passes, in the order they run. Change it through [BehaviorBus](BehaviorBus.md) rather than directly.

```csharp
bool HasPath { get; }
bool Completed { get; }
int CurrentPartIndex { get; }
DynamicState CurrentDynamicState { get; }
```

Whether there is a path, whether its end was reached, which curve the agent is on, and the latest state.

## Related

- [SAPass](SAPass.md) — what a pass is.
- [PathAgentModel](../../../Domain/Agent/PathAgentModel.md) — the state it reads and writes.
- [BehaviorBus](BehaviorBus.md) — swaps the passes at runtime.
- [PathAgentPipeline](PathAgentPipeline.md) — hands it new plans.
