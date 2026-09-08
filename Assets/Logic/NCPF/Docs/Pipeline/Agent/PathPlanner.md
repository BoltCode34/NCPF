# PathPlanner

The planning shell: a MonoBehaviour that wires the search brain, the behaviour bus and the feature container, drives the body from the model in FixedUpdate, and draws the plan. All planning logic lives in [[PathAgentPipeline]] (app); map composition in [[NcpfGraphComposer]]; feature wiring in [[PathFeatureContainer]].

`NCPF.Pipeline.Presentation · PathPlanner.cs`

## What it is

A thin shell with inspector fields organized into headers: Dependencies, Search, NCPF Preferences, Features, Runtime, Drawing, Model State. The model ([[PathAgentModel]]) is the single source of truth — the shell keeps no parallel copy of path/cursor state. It reads the model for diagnostics and drives the body transform from the model in FixedUpdate.

## Constructor / Wiring

```csharp
void Construct(PathAgentContext context, int attemptBudget)
```
Full re-wire from an externally built context (composition root calls this). No-op once wired.

```csharp
void InjectStandardDependency()
```
Inspector-fallback bootstrap: builds map/graph/finders and wires the brain when no composition root has injected a context. Reads ports from inspector fields.

```csharp
void RebuildMap()
```
Full re-wire: breaks the in-flight search, drops the plan and rebuilds from current inspector knobs. The body's state survives via a rebuild seed.

## Key API

```csharp
ITarget Target { get; set; }
```
Live target reference via [[TargetRefHolder]]. Set by [[ChainHead]] on follow.

```csharp
IGridMap3D Map / ICurvateGraph Graph / DynamicAgent Agent / PathAgentContext Context
```
Read-only views into the wired context.

```csharp
float RearGearPenalty / CurvaturePenalty / BaseTimePenalty
```
Preference knobs read by [[CurvateEdgeGenerator]] and [[RearGearGraph]].

```csharp
void SetAchivement(AchivementFeature feature) / SetHeuristic(HeuristicFeature feature) / SetMainPass(MainPlannerFeature feature)
```
Delegates to [[PathFeatureContainer]] — null falls back to [[PlannerStandards]].

```csharp
void AddSAOverride(PlannerOverrideFeature feature) / RemoveSAOverride(PlannerOverrideFeature feature)
```
Adds/removes an override pass on the bus.

## Diagnostics

13 `_dbg` fields in the inspector (Model State block): signed speed, cursor (index/arc/left), gear of current and next primitive. Populated by `LogModelState` from [[PathAgentModel]] each frame — no parallel state.

## Related

- [[PathAgentPipeline]] — the brain (app)
- [[PathFollowAgent]] — the follower (app)
- [[BehaviorBus]] — live behaviour slots (app)
- [[PlanRollout]] — edit-time sim (app)
- [[NcpfGraphComposer]] — DI composer (pres)
- [[PathFeatureContainer]] — feature→pass wiring (pres)
- [[PlannerStandards]] — standing features (pres)
- [[PathAgentContext]] — live dependencies
- [[TargetRefHolder]] — live target swap