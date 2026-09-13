# PlanRollout

Simulates driving a plan from start to finish without touching the real agent. Used to draw the **Simulate** preview on [PathPlanner](../../../Pipeline/Presentation/PathPlanner.md).

`NCPF.Pipeline.Application` · `PlanRollout.cs`

## Description

PlanRollout steps a separate [PathFollowAgent](PathFollowAgent.md) along a plan and records the state after every step. It does no drawing; the caller renders the recorded states.

Give it a follower built on its own agent model — [NcpfGraphComposer](../../../Pipeline/Presentation/NcpfGraphComposer.md) `CreateSimContext` creates a context for this — so the simulation never changes the real agent.

## Key API

```csharp
static List<DynamicState> Run(PathFollowAgent sim, float dt, int maxSteps)
```

Steps `sim` until it reaches the end of its plan or `maxSteps` is reached.

| Parameter | Purpose |
|---|---|
| `sim` | A separate follower, already started on the plan. |
| `dt` | The time step of each simulated step, in seconds. |
| `maxSteps` | The most steps to run, in case the plan never completes. |

Returns the state after each step.

## Related

- [PathFollowAgent](PathFollowAgent.md) — what is simulated.
- [PathPlanner](../../../Pipeline/Presentation/PathPlanner.md) — draws the result.
