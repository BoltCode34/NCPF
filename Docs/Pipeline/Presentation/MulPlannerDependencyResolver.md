# MulPlannerDependencyResolver

Groups planners by the data they have in common and builds each shared dependency once per group. Used by [NcpfPlannerBootstrap](NcpfPlannerBootstrap.md).

`NCPF.Pipeline.Presentation` · `MulPlannerDependencyResolver.cs`

## Description

Different dependencies depend on different inputs. The map depends only on the map asset; the search graph also depends on the agent's dynamics and preferences. So the resolver groups planners step by step, from the most general input to the most specific, and at each step builds only what that input allows it to share.

It is a chain of resolvers, one per level:

| Level | Resolver | Groups by | Builds once per group |
|---|---|---|---|
| Map | `MulPlannerDependencyResolver` → `SharedMapPlannerDependencyResolver` | map asset | the lattice map, [SpacedMap3D](../../Shared/SpacedMap3D.md) |
| Control set | `SharedSetPlannerDependencyResolver` | control set asset | the [DiscretizedControlSet3D](../../Domain/ControlSet/DiscretizedControlSet3D.md), the [PrimitivePassability](../../Domain/Space/PrimitivePassability.md) mask, [PlanGeometry](../../Pipeline/Application/Search/PlanGeometry.md) |
| Agent | `SharedAgentPlannerDependencyResolver` | dynamics asset + rear gear, curvature and base time penalties | the search graph and its cache |
| Planner | `UniquePlannerDependencyResolver` | — | what cannot be shared: the agent model, the target, the finders |

The last level builds a [PathAgentContext](../../Pipeline/Application/Agent/PathAgentContext.md) from all of this and calls `PathPlanner.Construct`.

Each level remembers the groups it has already built. A planner resolved later with the same inputs lands in an existing group and reuses its dependencies instead of building new ones.

## Key API

```csharp
void ResolveAll(IReadOnlyList<PathPlanner> planners)
```

Resolves every planner in the list.

```csharp
void Resolve(PathPlanner planner)
```

Resolves one planner: finds or builds its group at every level and injects the result. Planners with an unassigned grid, map, control set or dynamics asset are skipped and wire themselves as usual.

## Notes

- Agents that share a search graph also share its neighbour cache, so exploring an area for one agent makes it faster for the others.
- One map asset is assumed to be baked for one grid: planners are grouped by the map asset, and the grid is taken from the first planner of the group.

## Related

- [NcpfPlannerBootstrap](NcpfPlannerBootstrap.md) — runs the resolver when a scene starts.
- [NcpfGraphComposer](NcpfGraphComposer.md) — builds the same objects for a single planner.
- [CachedCurvateGraph](../../Pipeline/Application/Space/CachedCurvateGraph.md) — the cache shared at the agent level.
