# NCPF — Architecture

NCPF (Non-holonomic Curvature-aware Path Finder) is a geometric 3D search package: it bakes a
configuration-space control set over a grid, builds a curvature-aware graph of motion primitives
on top of it, and runs a signed-speed PV-ladder planner against that graph. The package is split
into nine assemblies across two verticals (Bake, Pipeline) over a shared domain, config and ports
layer.

## Assembly graph

```
                       NCPF  (domain, ns NCPF.Domain)
                     ▲   ▲   ▲   ▲
     NCPF.Bake.App ──┘   │   │   └── NCPF.Pipeline.App
           ▲              │   │              ▲
     NCPF.Bake.Pres ──▶ NCPF.Shared ◀── NCPF.Pipeline.Pres
                             ▲
                         NCPF.Config   (referenced by NOBODY)
     NCPF.Bake.Editor ─▶ Bake.Pres      NCPF.Pipeline.Editor ─▶ Pipeline.Pres
```

Arrows point from a referencing assembly to the one it references (downward only). Editor
assemblies are Editor-only platforms; everything else is runtime.

### Reference table

| Assembly | Namespace | References |
|---|---|---|
| `NCPF` | `NCPF.Domain` | Core.Foundation, PathFindingAbstract |
| `NCPF.Shared` | `NCPF.Shared` | NCPF, Features.Grid, Core.Foundation, PathFindingAbstract |
| `NCPF.Config` | `NCPF.Config` | NCPF, NCPF.Shared, Core.Foundation |
| `NCPF.Bake.Application` | `NCPF.Bake.Application` | NCPF, NCPF.Shared, Core.Foundation |
| `NCPF.Bake.Presentation` | `NCPF.Bake.Presentation` | NCPF.Bake.Application, NCPF, NCPF.Shared, Features.Grid |
| `NCPF.Bake.Editor` | `NCPF.Bake.Editor` *(Editor)* | NCPF.Bake.Presentation, NCPF.Bake.Application, NCPF, NCPF.Shared, Core.Foundation |
| `NCPF.Pipeline.Application` | `NCPF.Pipeline.Application` | NCPF, PathFindingAbstract, Features.PathFinding, Core.Foundation |
| `NCPF.Pipeline.Presentation` | `NCPF.Pipeline.Presentation` | NCPF.Pipeline.Application, NCPF, NCPF.Shared, Core.Foundation, Features.Grid, PathFindingAbstract, Features.PathFinding, Core.Shared |
| `NCPF.Pipeline.Editor` | `NCPF.Pipeline.Editor` *(Editor)* | NCPF.Pipeline.Presentation, NCPF.Pipeline.Application |

## Layer rules

- **Downward only (compiler-enforced).** An assembly references only those below it in the graph.
  No upward edges, no cycles.
- **Verticals are blind to each other (compiler-enforced).** Bake does not reference Pipeline and
  vice versa; both sit over the same domain and shared ports.
- **Application does not know presentation (convention).** App assemblies hold pure logic; they do
  not reference their Pres assembly. A Pres assembly references its App. One-namespace utility
  files in App are the exception, not a license to pull Pres.
- **Config is referenced by nobody.** `NCPF.Config` holds concrete asset types; no assembly
  references it. See [Config vs artifact](#config-vs-artifact).

## What lives where

- **NCPF (domain)** — pure geometry and state, no Unity outside `UnityEngine` math/structs: `IPath`
  and the path family (clothoid, rear-first, function/polynomial, bezier, discretized); `WorldConfig`,
  `Config`, `DynamicState`; `Map3D`/`AgentRig`/`Map`/`Rectangle`/`Agent`; `IGridMap3D`/`IGridGeometry3D`/
  `IGridLattice3D`; `ControlSet<T>`, `QuarterSymmetry`, `DiscretizedControlSet3D`; the agent model
  (`PathAgentModel`, `PrimitiveWindow`, `CurvatureSpeedProfile`, `PathGearUtility`, `PrimitiveAnalyzer`,
  `PathLimiterBase`); the ClothoidX vendor (`ThirdParty` namespace).
- **NCPF.Shared** — the storage ports: `ControlSetAsset`, `MapAsset`, `AgentConfig` (abstract
  `ScriptableObject` bases — `Read`/`Write`/`GetAgent`); `SpacedMap3D`, the shared grid adapter.
- **NCPF.Config** — concrete assets: `FuncControlSetContainer`, `MapContainer` (+ nested
  serialization DTOs), each with `[CreateAssetMenu]`.
- **NCPF.Bake.Application** — the bake chain: `ControlSetBuilder`/`IControlSetBuilder`,
  `LayerControlSetBuilderBase`, `BackwardLayerBuilder`, `ClothoidPathBuilder`/`RearFirstPathBuilder`,
  `ControlSetBakeService`, the build-log contract (`IControlSetBuildLogger`, `LayerBuilderLog`).
- **NCPF.Bake.Presentation** — `NCPFMapBuilder` (the MonoBehaviour that bakes a `MapAsset`),
  `RectangularConfig`, `ControlSetBuildLogger` SO, `ControlSetBaker` (the on-scene baker with
  ContextMenu + gizmos), `InlineFieldAttribute`.
- **NCPF.Bake.Editor** — `NCPFMapEditor`, `RectangularConfigEditor`, `InlineFieldDrawer`.
- **NCPF.Pipeline.Application** — the search and the agent: `CurvateMap3D`/`CurvateEdgeGenerator`/
  `ControlSetValidator3D`/`Projector3D`/`RearGearGraph`/`ICurvateGraph`; `PathFindingAgent`, the
  goal builders (`BaseGoalBuilder`, `IAsyncGoalBuilder` family), `EpsilonLadderFinder`; the one brain
  (`PathAgentPipeline`), the follower (`PathFollowAgent`), the behaviour slots (`BehaviorBus`),
  `PlanRollout`; `PathAgentContext`, the PV/AV space (`SpacedMapPV`/`ConfigPV`/…), `MotionWindow`,
  `FacingModes`.
- **NCPF.Pipeline.Presentation** — the planner shell `PathPlanner`; `NcpfGraphComposer` (dependency
  injection: builds map → graph → finders → context), `PathFeatureContainer` (feature→pass
  reconciliation), `PlannerStandards` (the three standing features); the `[Serializable]` feature
  family (`HeuristicFeature`/`AchivementFeature`/`MainPlannerFeature`/`PlannerOverrideFeature` and
  concrete features); `DynamicAgentConfig`, `SubclassSelectorAttribute`, `DynamicsVisualizer`.
- **NCPF.Pipeline.Editor** — `SubclassSelectorDrawer`.

## Config vs artifact

A concrete asset (`MapContainer`, `FuncControlSetContainer`) is an **artifact**; the abstract
storage base (`MapAsset`, `ControlSetAsset`) is the **port**. The port lives in `NCPF.Shared`;
the concrete lives in `NCPF.Config`. **No assembly references `NCPF.Config`.** Unity binds an
asset to its concrete type by the script `.meta` GUID at import time; a field typed as the
abstract port holds the concrete asset at runtime, and the virtual `Read`/`Write` dispatches to
the concrete. This keeps the dependency arrow pointing at the port, never at the concrete:
- Composition roots and Pres fields are typed `MapAsset`/`ControlSetAsset`/`AgentConfig`.
- Creating an asset is a `[CreateAssetMenu]` on the concrete (in Config); writing it goes through
  the abstract `Write`.
- Adding a new concrete storage format means a new type in `NCPF.Config` — nothing else recompiles.

## Ports and adapters

The ports/adapters seam runs through `NCPF.Shared` and `NCPF.Bake.Application`:
- **Storage ports** (`NCPF.Shared`): `MapAsset.Read()→Map3D`, `ControlSetAsset.Read(IGridGeometry3D)→ControlSet<IPath>`, `AgentConfig.GetAgent(cellSize)→Agent`. Adapters are the Config concretes.
- **Process contract** (`NCPF.Bake.Application`): `IControlSetBuildLogger` is the contract; the SO logger in `NCPF.Bake.Presentation` is the adapter. `ControlSetBakeService` speaks only to the contract.

`NcpfGraphComposer` (Pipeline.Presentation) is the pipeline's adapter from storage ports to a live
`PathAgentContext`: it reads the ports, builds the `CurvateMap3D`/`RearGearGraph`/finders and hands
the context to the shell. The shell and the composition root (`SuckerResolver`) wire the brain
from that context; the brain itself depends only on app-level interfaces.

## The model is the truth

State lives in `PathAgentModel` (domain) and the `PathFollowAgent` cursor. The shell keeps no
parallel copy of path/cursor state — it reads the model for diagnostics (`LogModelState`) and
drives the body transform from the model in `FixedUpdate`. Services carry no agent state; the
brain owns the search and feeds the follower, never the transform.

## Phase 2 disciplines (not yet applied)

Phase 1 produced clean assembly boundaries and a single composition root per vertical. Phase 2
hardens them:
- **Internal-by-default.** Surface area inside each assembly defaults to `internal`; a whitelist
  of genuinely public types (the ports, the context, the shell) is the public API. Today everything
  is `public` — phase 2 narrows it.
- **Editor-only enforcement.** Editor assemblies already target the Editor platform; phase 2 also
  moves any editor-only utility out of runtime assemblies (no `#if UNITY_EDITOR` in runtime code).
- **No hardcoded paths.** The JSON fallback in `NCPF.Config` uses a hardcoded path; phase 2 routes
  it through an injected location or removes the fallback.
- **Composition facade.** `NcpfGraphComposer` and `SuckerResolver` are the two composition roots
  today; phase 2 may consolidate behind a single facade so callers wire dependencies through one
  entry point.
- **Bake out of runtime.** Bake currently ships in the runtime build; phase 2 moves bake to an
  editor/authoring-only surface so a player build carries no bake code.

## File discipline

Every source file in the package follows: no commented-out code; no debug output of any kind;
no inline comments — only XML `///` summaries on significant types, English, Unity-API tone, ≤4
lines per type and ≤2–3 per major public method; one type per file (small related interface
families excepted); user code blocks always use newlines and indentation, never one-liner braces.
A mechanical gate enforces zero Cyrillic (including string literals), zero `//`-comments and zero
`TODO`/`HACK` across `Assets/Game/NCPF/**/*.cs`.
