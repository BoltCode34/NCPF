# PathFeatureContainer

Owns the feature-to-pass wiring and the dirty-work around inspector features: dedupes a changed feature against a buffer, substitutes a standing standard for an empty slot, materializes the pass/builder on the live [[BehaviorBus]], and adds/removes override passes as the override list changes. Created fresh on each (re)construct so the buffers and the feature→pass map start clean.

`NCPF.Pipeline.Presentation · PathFeatureContainer.cs`

## What it is

The feature reconciliation layer. The [[PathPlanner]] shell holds `[SerializeReference]` feature slots; this container watches them, materializes the right pass/builder on the [[BehaviorBus]], and handles null→standard fallback.

## Constructor

```csharp
PathFeatureContainer(PlannerStandards standards)
```

`standards` — the [[PlannerStandards]] instance holding the three standing features (point-cell achievement, direct-distance heuristic, travel main planner).

## Key API

```csharp
void Bind(BehaviorBus bus, PathAgentContext context,
          MainPlannerFeature main, AchivementFeature achivement, HeuristicFeature heuristic,
          List<PlannerOverrideFeature> overrides)
```
Reconciles the live features with the bus:
- Changed `main` / `achivement` / `heuristic` → materialize pass/builder on bus (null → standard from `standards`).
- New override in `overrides` → `CreatePass(context)` → `bus.AddOverride`.
- Removed override → `bus.RemoveOverride` → drop from internal map.

No-ops when nothing changed.

## Details

Buffers track the last-bound feature instance per slot (`_mainBuf`, `_achivementBuf`, `_heuristicBuf`, `_overridesBuf`) plus a `_passes` dict for override→pass mapping. This makes `Bind` idempotent and safe to call every frame (which [[PathPlanner]].`Update` does).

All feature logic stays in [[NCPF.Pipeline.Presentation]] — the bus and follower in `NCPF.Pipeline.Application` know nothing of features.

## Related

- [[BehaviorBus]] — the target (app)
- [[PlannerStandards]] — standing features (pres)
- [[PathPlanner]] — the shell that owns features and calls Bind (pres)
- [[MainPlannerFeature]] / [[AchivementFeature]] / [[HeuristicFeature]] / [[PlannerOverrideFeature]] — feature bases
- [[PathAgentContext]] — what CreatePass/CreateBuilder receives