# PathPlanner

The component you put on an agent to make it plan a path to a target and drive along it. It is the entry point for everything NCPF does at runtime.

`NCPF.Pipeline.Presentation` · `PathPlanner.cs`

## Description

PathPlanner is a thin shell. It holds the Inspector configuration, builds the runtime objects from it, and then delegates the actual work:

- planning — when to search, when to replan, how a new plan replaces the old one — to [PathAgentPipeline](../../Pipeline/Application/Agent/PathAgentPipeline.md);
- driving along the plan to [PathFollowAgent](../../Pipeline/Application/Agent/PathFollowAgent.md);
- building the map, graph and finders to [NcpfGraphComposer](NcpfGraphComposer.md);
- turning the Inspector features into live behaviour to [PathFeatureContainer](PathFeatureContainer.md).

Each frame, `Update` runs the planning loop and `FixedUpdate` moves the Transform to the agent's new pose. The agent does not need a Rigidbody.

PathPlanner runs in Edit mode too (`[ExecuteAlways]`), so you can see a plan in the Scene view without entering Play mode.

## Inspector

### Dependencies

| Field | Purpose |
|---|---|
| **Grid** | The [Unity2DGrid](../../Shared/Unity2DGrid.md) the map was baked on. Converts between world positions and cells. |
| **Map Container** | The baked map, a [MapAsset](../../Shared/MapAsset.md). |
| **Control Set Container** | The baked primitive library, a [ControlSetAsset](../../Shared/ControlSetAsset.md). |
| **Dynamic Agent** | The agent's speed, acceleration and turn-rate limits, a [DynamicAgentConfig](DynamicAgentConfig.md). |
| **Target** | The Transform the agent chases. |
| **Agent Shape** | An [AgentConfig](../../Shared/AgentConfig.md) used only to draw the body in the simulated preview. It does not affect planning: the shape used for planning is the one the map was baked with. |

### Search

| Field | Purpose |
|---|---|
| **Epsilon Ladder** | The heuristic weights the search tries, in order. See [EpsilonLadderFinder](../../Pipeline/Application/Search/EpsilonLadderFinder.md). |
| **Attempt Budget** | How many nodes one search may expand before it gives up. |
| **Replan Interval** | The minimum time between two searches, in seconds. |

### NCPF Preferences

These shape which paths the search prefers. They change edge costs, not what is possible.

| Field | Purpose |
|---|---|
| **Rear Gear Penalty** | Multiplies the cost of every reversing primitive. `1` makes reversing as cheap as driving forward; higher values keep reversing for when it is really needed. See [RearGearGraph](../../Pipeline/Application/Space/RearGearGraph.md). |
| **Curvature Penalty** | An extra cost for tight curves. `0` is neutral; around `0.1` makes a tight arc lose to a gentle one that turns just as far. |
| **Base Time Penalty** | How much of an edge's cost is its travel time. Keep it at `1`: lower values make the search's distance estimate overestimate, and the search can return worse paths. |

### Features

The behaviour of the agent, picked from a dropdown. An empty slot falls back to the standard feature from [PlannerStandards](PlannerStandards.md). See [Planner features](PlannerFeatures.md).

| Field | Purpose |
|---|---|
| **Achivement Feature** | When the target counts as reached. |
| **Heuristic Feature** | How the search estimates the remaining distance. |
| **Main Planner Feature** | How the agent chooses its speed and facing along the path. |
| **Overrides** | Optional behaviours applied on top of the main planner. |

### Runtime and Drawing

| Field | Purpose |
|---|---|
| **Time Scale** | Multiplies the time step the agent drives with. |
| **Draw** | Draws the current plan in the Scene view. |
| **Draw Points** / **Step Count** | Draws sample points along the plan, and how many. |
| **Simulate** / **Sim Dt** / **Sim Max Steps** / **Body Every** | Draws a simulated drive along the plan, with the body outline every *Body Every* steps. |

The **Model State** block at the bottom is read-only diagnostics mirrored from the agent model in Play mode.

## Key API

```csharp
void Construct(PathAgentContext context, int attemptBudget)
```

Wires the planner from a context built outside, instead of from its own Inspector fields. This is the hook for your own dependency injection, and it is what [NcpfPlannerBootstrap](NcpfPlannerBootstrap.md) uses to share dependencies between agents. Call it before the planner's `Start`: if you call it later, the planner has already built its own dependencies, and that work is thrown away.

| Parameter | Purpose |
|---|---|
| `context` | Everything the planner needs: model, map, graph, finders, target. See [PathAgentContext](../../Pipeline/Application/Agent/PathAgentContext.md). |
| `attemptBudget` | How many nodes one search may expand. |

```csharp
void InjectStandardDependency()
```

Builds the planner from its own Inspector fields. It runs automatically in `Start` when nothing was injected; call it yourself only if you need the planner ready earlier. Does nothing once the planner is wired. Also available from the component's context menu.

```csharp
void RebuildMap()
```

Throws away the current map, graph and plan and builds them again from the Inspector fields. The agent keeps its current speed and pose. The planner calls this on its own when you change a dependency or a preference in the Inspector. Also available from the component's context menu.

```csharp
ITarget Target { get; set; }
```

The target being chased. Setting it retargets the running planner without rebuilding anything. See [ITarget](../../Pipeline/Application/Agent/ITarget.md).

```csharp
void SetMainPass(MainPlannerFeature feature)
void SetAchivement(AchivementFeature feature)
void SetHeuristic(HeuristicFeature feature)
```

Replaces a feature from code. Passing `null` restores the standard one. The change takes effect on the next `Update`, while the agent keeps driving.

```csharp
void AddSAOverride(PlannerOverrideFeature feature)
void RemoveSAOverride(PlannerOverrideFeature feature)
```

Adds or removes an optional behaviour. Takes effect on the next `Update`.

```csharp
PathAgentContext Context { get; }
IGridMap3D Map { get; }
ICurvateGraph Graph { get; }
DynamicAgent Agent { get; }
```

Read-only views of the wired dependencies. `null` until the planner is wired.

```csharp
NcpfGraphComposer.PlannerConfig Config { get; }
```

The Inspector configuration as one value, read before anything is built. Used to group planners that can share dependencies.

## Notes

- **Changing the Inspector rebuilds.** If a dependency or preference no longer matches what the planner was built with, it calls `RebuildMap` on the next frame. A planner that was sharing dependencies with other agents then builds its own copy and stops sharing.
- The planner wires itself only when **Grid**, **Map Container** and **Control Set Container** are all set.

## Related

- [PathAgentPipeline](../../Pipeline/Application/Agent/PathAgentPipeline.md) — the planning loop it runs.
- [PathFollowAgent](../../Pipeline/Application/Agent/PathFollowAgent.md) — drives the agent along the plan.
- [NcpfGraphComposer](NcpfGraphComposer.md) — builds its dependencies.
- [PathFeatureContainer](PathFeatureContainer.md) — turns features into live behaviour.
- [NcpfPlannerBootstrap](NcpfPlannerBootstrap.md) — shares dependencies between planners.
- [Getting Started](../../GettingStarted.md) — setting one up.
