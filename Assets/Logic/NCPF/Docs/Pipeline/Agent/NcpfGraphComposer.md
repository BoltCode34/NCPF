# NcpfGraphComposer

Dependency injection for the pipeline: builds the search's data side from the inspector ports — map → discretized control set → [[CurvateMap3D]] → [[RearGearGraph]] → ε-ladder finders, packed into a [[PathAgentContext]]. Also tracks a signature of the knobs the planner was wired from, so the shell can detect an inspector change and rebuild.

`NCPF.Pipeline.Presentation · NcpfGraphComposer.cs`

## What it is

The pipeline's adapter from storage ports to a live [[PathAgentContext]]. The shell calls `Build` when no composition root wired it, and `CreateSimContext` for the edit-time rollout.

## Key API

```csharp
PathAgentContext Build(PlannerConfig config, ITarget target)
```
Fresh context from current inspector config:
1. `SpacedMap3D` from `config.Grid` + `config.Map.Read()`
2. `ControlSet<IPath>` from `config.ControlSet.Read(map3D)`
3. `DiscretizedControlSet3D` via `DiscretizedControlSet3DConverter`
4. `CurvateMap3D` with `CurvateEdgeGenerator` over `ControlSetValidator3D(Projector3D(...))`
5. `RearGearGraph` wrapping it with `config.RearGearPenalty`
6. `EpsilonLadderFinder` over `CurvateGoalPathFinder` with `config.EpsilonLadder` / `config.AttemptBudget`
7. `PathAgentModel` + context assembly

```csharp
void CaptureSignature(PlannerConfig config)
```
Records the current config as the wired signature (for a context the shell did not build here, e.g. one injected by the composition root).

```csharp
bool SignatureChanged(PlannerConfig config)
```
True if the signature was captured and the current config differs from it.

```csharp
PathAgentContext CreateSimContext(PathAgentContext live)
```
A throwaway context for edit-time rollout: a fresh model over the live map, graph, finders, target and agent. The simulation follower runs on it without touching the live model.

## PlannerConfig

The inspector-side knobs and ports the planner was wired from:
- `Unity2DGrid Grid`, `MapAsset Map`, `ControlSetAsset ControlSet`, `DynamicAgentConfig DynamicAgent`
- `float RearGearPenalty`, `CurvaturePenalty`, `BaseTimePenalty`
- `int AttemptBudget`, `float[] EpsilonLadder`

`Same(other)` does reference equality on the asset fields and value equality on the knobs.

## Related

- [[PathPlanner]] — the shell that calls Build/CaptureSignature/SignatureChanged/RebuildMap (pres)
- [[PathAgentContext]] — the live dependency bundle (app)
- [[SpacedMap3D]] — shared grid adapter (shared)
- [[DiscretizedControlSet3DConverter]] — discretization (app)
- [[CurvateEdgeGenerator]] / [[ControlSetValidator3D]] / [[Projector3D]] — edge chain (app)
- [[RearGearGraph]] — gear decorator (app)
- [[EpsilonLadderFinder]] / [[CurvateGoalPathFinder]] — finders (app)