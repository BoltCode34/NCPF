# PathAgentContext

Everything a planner needs, in one object: the agent model, its dynamics, the map, the graph, the finders, the target and the plan geometry.

`NCPF.Pipeline.Application` · `PathAgentContext.cs`

## Description

A context is what `PathPlanner.Construct` takes, and what features receive when they create their passes and builders. Building one yourself is how you give a planner dependencies from your own setup.

Most of a context can be shared between agents that use the same data. The `Model` and the `Target` belong to one agent and must not be shared. See [MulPlannerDependencyResolver](../../../Pipeline/Presentation/MulPlannerDependencyResolver.md).

## Constructor

```csharp
PathAgentContext(PathAgentModel model, DynamicAgent dynamicAgent,
                 IAsyncGoalPathFinder<TransitionData> asyncPathFinder,
                 IGoalPathFinder<TransitionData> pathFinder,
                 IGridMap3D grid, ICurvateGraph graph, ITarget target, PlanGeometry geometry)
```

| Parameter | Purpose |
|---|---|
| `model` | This agent's path, position along it and state. One per agent. |
| `dynamicAgent` | Speed, acceleration and turn-rate limits. |
| `asyncPathFinder` | Runs the main search on a background thread. |
| `pathFinder` | Runs short searches on the main thread, such as stitching a new plan onto the current one. |
| `grid` | Converts between world poses and lattice nodes. |
| `graph` | The lattice the search walks. |
| `target` | What this agent chases. One per agent. |
| `geometry` | Turns a searched chain of edges back into curves. |

## Related

- [PathPlanner](../../../Pipeline/Presentation/PathPlanner.md) — `Construct` takes a context.
- [NcpfGraphComposer](../../../Pipeline/Presentation/NcpfGraphComposer.md) — the standard way to build one.
- [Planner features](../../../Pipeline/Presentation/PlannerFeatures.md) — features read it.
