# SAPass

A pass: one step of the decision about how an agent moves this frame. Passes decide a target speed and a target facing; they never move the agent.

`NCPF.Pipeline.Application` · `PathAgentOverrider.cs`

## Description

[PathFollowAgent](PathFollowAgent.md) runs passes every step. The **main pass** makes the first decision from the agent's state. **Override passes** then adjust it, one after another. Only after all of them does the follower move the agent.

A pass receives the state read-only and returns a `ControlInput`. That asymmetry is the contract: passes decide, the follower moves.

### ControlInput

```csharp
public struct ControlInput
{
    public float TargetSpeed;   // metres per second along the path
    public float TargetFacing;  // degrees, 0 = +Y, counter-clockwise
}
```

- **TargetSpeed** is signed. Positive drives the path nose-first, negative drives it tail-first. The agent advances along the path by its magnitude.
- **TargetFacing** is where the body points. It is independent of the direction of travel: the agent can look one way and move another.

Both values are this step's targets, already limited by whoever produced them. The follower applies them as they are.

## Pass types

| Type | Decides | Implement |
|---|---|---|
| `MainSAPass` | Speed and facing together, from the state alone. Every agent has exactly one. | `ControlInput PlanControl(float dt, DynamicState current)` |
| `SAPass` | Anything, given the decision so far. | `ControlInput Override(float dt, DynamicState current, ControlInput input)` |
| `BaseAPass` | **Facing only.** Speed passes through untouched. | `float OverrideAngle(float dt, DynamicState current, ControlInput input)` |
| `BaseSPass` | **Speed only.** Facing passes through untouched. | `float OverrideSpeed(float dt, DynamicState current, ControlInput input)` |

Prefer `BaseAPass` or `BaseSPass` for an override that changes one channel. A pass that computed how far the agent can turn did so for a particular speed; quietly changing the speed in the same pass would make that calculation wrong.

## Key API

```csharp
ControlInput Override(float dt, DynamicState current, ControlInput input)
```

| Parameter | Purpose |
|---|---|
| `dt` | The time step, in seconds. |
| `current` | The agent's state, as the model last recorded it. |
| `input` | What the passes before this one decided. |

Returns the decision after this pass.

## Related

- [PathFollowAgent](PathFollowAgent.md) — runs the passes.
- [LookPass](LookPass.md) — a ready-made facing pass.
- [Planner features](../../../Pipeline/Presentation/PlannerFeatures.md) — how passes are chosen in the Inspector.
- [BehaviorBus](BehaviorBus.md) — adds and removes passes at runtime.
