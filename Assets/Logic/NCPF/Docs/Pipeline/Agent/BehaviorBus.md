# BehaviorBus

Live behaviour slots the planner swaps without rebuilding the pipeline: the main pass and the two goal-builder halves sit behind ref decorators, so changing a feature re-points the decoratee while the search and the follower keep running. Override passes are added/removed on the follower's pass list. The bus speaks only in app terms — it knows nothing of features.

`NCPF.Pipeline.Application · Agent/BehaviorBus.cs`

## What it is

The behavioural indirection layer. The [[PathPlanner]] shell deals in features ([[AchivementFeature]], [[HeuristicFeature]], [[MainPlannerFeature]], [[PlannerOverrideFeature]]); the bus converts them to app-level passes/builders and wires them onto the follower. The chicken-and-egg (follower needs `Main` at ctor; bus needs follower for override list) is broken by a parameterless ctor + `Attach(follower)`.

## Key API

```csharp
RefMainPlanner Main { get; }
RefHeuristicBuilder Heuristic { get; }
RefAchivementBuilder Achivement { get; }
```
Ref decorators with live `Decoratee` fields. Swapping a feature means assigning a new decoratee — the search and follower see the change immediately.

```csharp
void Attach(PathFollowAgent follower)
```
Call once, before the first `AddOverride`. The ref slots are already live and may feed the follower's ctor.

```csharp
void SetMain(MainSAPass pass) / SetAchivement(IAchivementBuilder builder) / SetHeuristic(IHeuristicBuilder builder)
```
Assign the decoratee on the corresponding ref.

```csharp
void AddOverride(SAPass pass) / RemoveOverride(SAPass pass)
```
Mutates the follower's `Passes` list. Null/duplicate/guard-checked. The bus's internal `_overrides` tracks what was added so `RemoveOverride` can find it.

```csharp
IReadOnlyList<SAPass> Overrides
```
The current override list (for inspection).

## Ref Types

- `RefMainPlanner : MainSAPass` — forwards `PlanControl` to `Decoratee`.
- `RefHeuristicBuilder : IHeuristicBuilder` — forwards sync `Create`; async `Create` casts `Decoratee` to `IAsyncHeuristicBuilder` so a heavy heuristic (Dijkstra) never falls back to the sync default.
- `RefAchivementBuilder : IAchivementBuilder` — mirrors the heuristic ref.

## Related

- [[PathFollowAgent]] — whose pass list is mutated (app)
- [[PathAgentPipeline]] — brain that owns the bus (app)
- [[PathFeatureContainer]] — materializes features onto the bus (pres)
- [[MainSAPass]] / [[IHeuristicBuilder]] / [[IAchivementBuilder]] — app contracts
- [[IAsyncHeuristicBuilder]] / [[IAsyncAchivementBuilder]] — async surfaces