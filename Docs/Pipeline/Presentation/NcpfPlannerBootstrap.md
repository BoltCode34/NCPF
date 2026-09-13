# NcpfPlannerBootstrap

Makes planners that use the same data share it. It runs automatically when a scene starts in Play mode, before any planner builds anything.

`NCPF.Pipeline.Presentation` · `NcpfPlannerBootstrap.cs`

## Description

Without it, every [PathPlanner](PathPlanner.md) builds its own map, control set, passability mask and search graph — even when ten agents use exactly the same assets. The passability mask alone takes a noticeable moment to bake, so a scene with many agents would spend most of its start-up time building identical copies.

When a scene loads, the bootstrap finds every PathPlanner in it and passes them to a [MulPlannerDependencyResolver](MulPlannerDependencyResolver.md), which builds each dependency once per group of planners that can share it and injects the result through `PathPlanner.Construct`.

It runs after every object's `Awake` but before any `Start`. A planner that has already received a context does not build its own in `Start`, so nothing is built twice.

Sharing is controlled by [NCPFSettings](../../Config/NCPFSettings.md). With no settings asset in a `Resources` folder, sharing is on.

## Key API

```csharp
static void Resolve(PathPlanner planner)
```

Adds a planner created after the scene started — for example, a spawned enemy — to the group it can share with. Call it right after adding the PathPlanner, before its `Start` runs. Does nothing when sharing is switched off.

| Parameter | Purpose |
|---|---|
| `planner` | The newly created planner. |

## Notes

- **Play mode only.** In Edit mode the bootstrap does not run, and each planner builds its own dependencies.
- **Planners must be in the scene when it loads** to be picked up automatically. Anything created later needs `Resolve`.
- **A planner leaves its group when its Inspector changes.** If a dependency or preference no longer matches what it was built with, the planner rebuilds its own copy.
- Shared data is released when the scene unloads.

## Related

- [MulPlannerDependencyResolver](MulPlannerDependencyResolver.md) — does the grouping and building.
- [NCPFSettings](../../Config/NCPFSettings.md) — switches sharing on and off.
- [PathPlanner](PathPlanner.md) — the component whose dependencies are shared.
