# DynamicAgentConfig

An asset holding an agent's physical limits: top speed, acceleration and turn rate.

`NCPF.Pipeline.Presentation` · `DynamicAgentConfig.cs`

## Description

Create one with **Create > NCPF > Configs > DynamicAgentConfig** and assign it to the **Dynamic Agent** field of a [PathPlanner](PathPlanner.md). The search uses these limits to price curves — a tight curve forces a lower speed and so takes longer — and the main planner uses them to decide how fast the agent actually drives.

## Inspector

The limits are under the **Agent** foldout.

| Field | Purpose |
|---|---|
| **Max Velocity** | Top speed, in metres per second. |
| **Max Linear Acceleration** | How quickly the agent speeds up and brakes, in metres per second squared. |
| **Max Angled Velocity** | How quickly the agent can turn its body, in degrees per second. |

## Key API

```csharp
DynamicAgent Agent { get; }
```

The limits as a [DynamicAgent](../../Domain/Core/DynamicAgent.md). A new instance is created on every read, so changes made in the Inspector apply immediately.

## Notes

- Planners that share a DynamicAgentConfig asset — and the same preferences — share one search graph. Two assets with identical values still count as different. See [MulPlannerDependencyResolver](MulPlannerDependencyResolver.md).

## Related

- [DynamicAgent](../../Domain/Core/DynamicAgent.md) — the limits themselves.
- [PathPlanner](PathPlanner.md) — where it is assigned.
- [DynamicsVisualizer](DynamicsVisualizer.md) — shows what these limits look like in motion.
