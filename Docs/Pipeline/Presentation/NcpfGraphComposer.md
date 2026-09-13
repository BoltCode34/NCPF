# NcpfGraphComposer

Builds everything a planner needs from its Inspector configuration: the map, the control set, the search graph and the finders, packed into a [PathAgentContext](../../Pipeline/Application/Agent/PathAgentContext.md).

`NCPF.Pipeline.Presentation` · `NcpfGraphComposer.cs`

## Description

NcpfGraphComposer is the standard dependency setup of a [PathPlanner](PathPlanner.md). The planner uses it whenever nothing outside has injected a context. It reads the baked assets through their ports, prepares the control set for the grid, bakes the passability mask, and assembles the search graph:

```
CachedCurvateGraph
 └─ RearGearGraph
     └─ CurvateMap3D
         └─ PassabilityTransitionGenerator
```

It also remembers the configuration it built from, so the planner can tell when the Inspector has changed and a rebuild is due.

When many planners share dependencies, [MulPlannerDependencyResolver](MulPlannerDependencyResolver.md) builds the same objects instead, once per group of planners.

## Key API

```csharp
PathAgentContext Build(PlannerConfig config, ITarget target)
```

Builds a fresh context and records `config` as the one it was built from.

| Parameter | Purpose |
|---|---|
| `config` | The planner's configuration: assets, preferences, search settings. |
| `target` | What the agent chases. |

Returns a context with a new agent model and freshly built map, graph and finders.

```csharp
bool SignatureChanged(PlannerConfig config)
```

Returns `true` if a configuration was recorded and `config` differs from it. The planner calls this every frame to decide whether to rebuild.

```csharp
void CaptureSignature(PlannerConfig config)
```

Records `config` without building anything. Used when a context was injected from outside, so an unchanged Inspector does not trigger a rebuild.

```csharp
PathAgentContext CreateSimContext(PathAgentContext live)
```

Creates a throwaway context that shares the live one's map, graph and finders but has its own agent model. Used to simulate a drive for the preview without disturbing the real agent.

## PlannerConfig

A value describing everything a planner is built from.

| Field | Meaning |
|---|---|
| `Grid`, `Map`, `ControlSet`, `DynamicAgent` | The scene grid and the assets. |
| `RearGearPenalty`, `CurvaturePenalty`, `BaseTimePenalty` | The cost preferences. |
| `AttemptBudget`, `EpsilonLadder` | The search settings. |

`Same(PlannerConfig other)` compares the grid and assets by reference and everything else by value.

## Related

- [PathPlanner](PathPlanner.md) — the component it builds for.
- [MulPlannerDependencyResolver](MulPlannerDependencyResolver.md) — builds the same objects shared between planners.
- [PathAgentContext](../../Pipeline/Application/Agent/PathAgentContext.md) — what it returns.
- [PassabilityBaker](../../Pipeline/Application/Space/PassabilityBaker.md) — bakes the collision mask during the build.
