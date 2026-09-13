# PathAgentPipeline

The planning loop of one agent. Every frame it decides whether the agent needs a new plan, starts searches, and swaps a finished plan in while the agent keeps moving.

`NCPF.Pipeline.Application` · `PathAgentPipeline.cs`

## Description

PathAgentPipeline owns the search ([PathFindingAgent](../Search/PathFindingAgent.md)) and the driver ([PathFollowAgent](PathFollowAgent.md)). [PathPlanner](../../../Pipeline/Presentation/PathPlanner.md) calls `Update` every frame with the body's current pose. The pipeline never touches a Transform.

### What Update does

1. **Is the plan still good?** A plan is kept while it ends within two cells of the target. If it is, nothing else happens.
2. **Is a search running?** If so, wait for it.
3. **Is a result waiting?** If so, set it up: stitch it onto the path being driven and hand it to the follower.
4. **Otherwise**, if at least **Replan Interval** seconds have passed since the last search, start a new one from the body's pose to the target.

In Edit mode the interval is measured in real time, so replanning works without Play mode.

### Hot and cold swaps

When a result arrives while the agent already has a path, it is a **hot swap**: the new path is stitched onto the curve the agent is currently on, and the agent keeps its speed and heading. There is no jump.

When the agent has no path yet, it is a **cold start**: the follower starts from the body's current state. If the planner was just rebuilt, it starts from the state saved before the rebuild instead, so rebuilding does not stop the agent.

## Constructor

```csharp
PathAgentPipeline(PathFindingAgent search, PathFollowAgent follower, IGridMap3D map,
                  ITarget target, float replanInterval, DynamicState? rebuildSeed)
```

| Parameter | Purpose |
|---|---|
| `search` | Finds and stitches paths. |
| `follower` | Drives the agent along the plan. |
| `map` | Used to judge whether the plan still ends close enough to the target. |
| `target` | What the agent chases. |
| `replanInterval` | The minimum time between searches, in seconds. |
| `rebuildSeed` | The state to resume from after a rebuild, or `null`. |

## Key API

```csharp
void Update(DynamicState bodyState)
```

Runs one step of the loop.

| Parameter | Purpose |
|---|---|
| `bodyState` | The body's current pose. Used as the start of a new search and as the seed of a cold start. |

```csharp
void Break()
```

Abandons the running search and drops the plan. The next `Update` plans from scratch.

```csharp
CurvateTransition[] Path { get; }
PathFollowAgent Follower { get; }
```

The current plan as real curves, and the follower driving it.

## Related

- [PathFindingAgent](../Search/PathFindingAgent.md) — the search it runs.
- [PathFollowAgent](PathFollowAgent.md) — the driver it feeds.
- [PathAgentModel](../../../Domain/Agent/PathAgentModel.md) — where the swap rules live.
- [PathPlanner](../../../Pipeline/Presentation/PathPlanner.md) — the component that calls it.
