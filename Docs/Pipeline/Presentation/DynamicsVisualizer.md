# DynamicsVisualizer

A Scene view tool that shows what an agent's dynamics limits look like, without a map or a planner. Use it to tune a [DynamicAgentConfig](DynamicAgentConfig.md) before putting it on an agent.

`NCPF.Pipeline.Presentation` · `DynamicsVisualizer.cs`

## Description

Add it to any GameObject. It draws three independent gizmos, in Edit mode and in Play mode:

1. **Straight ramp** — the agent accelerating from point A, cruising and braking to a stop at point B, coloured by speed, with a marker moving at the real pace. It shows how much of a straight is spent accelerating and braking.
2. **In-place spin** — the body outline rotating around its anchor at the maximum turn rate.
3. **Kinematic circle** — the circle an agent moving at a given speed and turn rate would drive. Its radius is speed divided by turn rate.

## Inspector

| Field | Purpose |
|---|---|
| **Dynamic Agent** | The limits to visualise. |
| **Agent Shape** | The body outline, for the spin. |
| **Draw Profile** / **Point A** / **Point B** | Enables the straight ramp and sets its ends. |
| **Segments** | How finely the ramp is drawn. |
| **Animate Marker** | Moves a marker along the ramp at the real pace. |
| **Draw Spin** | Enables the in-place spin. |
| **Draw Circle** / **Circle Speed** / **Circle Turn Speed** | Enables the kinematic circle, with its own speed (m/s) and turn rate (°/s). |

## Related

- [DynamicAgentConfig](DynamicAgentConfig.md) — the limits it shows.
- [AgentConfig](../../Shared/AgentConfig.md) — the body shape it draws.
