# Architecture

This page explains how NCPF is put together: what the main parts are, how they talk to each other, and why the package is split the way it is. It is written for someone who wants to understand the package or extend it. To just use it, [Getting Started](GettingStarted.md) is enough.

## The idea in one paragraph

NCPF never plans on raw grid cells. It first builds a **state lattice**: a grid where a position is a cell *and a heading*, and where two positions are connected only if the agent can actually drive from one to the other. The connections come from a small library of drivable curves — the **control set** — that is repeated at every cell. Whether the body fits along a curve comes from a precomputed **map** of where the body shape fits at each heading. Searching this lattice with A\* can only ever return a chain of drivable curves that the real body fits along, so there is no smoothing or fixing afterwards.

The approach follows Pivtoraiko & Kelly, *Generating Near Minimal Spanning Control Sets for Constrained Motion Planning in Discrete State Spaces*. NCPF adapts it to arbitrary non-circular body shapes.

## Two halves: bake and pipeline

Everything in NCPF belongs to one of two stages of the same data.

```
         EDITOR (once)                                   PLAY MODE (every frame)
┌──────────────────────────────┐                ┌───────────────────────────────────┐
│ BAKE                         │                │ PIPELINE                          │
│                              │   baked assets │                                   │
│ obstacles + body  ──► map    │ ─────────────► │ map + control set ──► search      │
│ curves            ──► set    │                │ plan              ──► drive       │
│                              │                │                                   │
└──────────────────────────────┘                └───────────────────────────────────┘
```

- **Bake** *produces* data. It runs in the Editor, when you press **Build** on [NCPFMapBuilder](Bake/Presentation/NCPFMapBuilder.md) or **Bake** on [ControlSetBaker](Bake/Presentation/ControlSetBaker.md). Its results are assets.
- **Pipeline** *consumes* that data. It runs in Play mode, inside [PathPlanner](Pipeline/Presentation/PathPlanner.md), and never changes the baked assets.

The two halves do not know about each other in code. They meet only through the baked assets. This is what lets you bake once and ship, and it is why a bug in the search can never corrupt a map.

## Three layers inside each half

Across both halves, code is also split by *what kind of question it answers*.

| Layer | Answers | Contains | Knows about Unity? |
|---|---|---|---|
| **Domain** | *What is this, and what must always hold?* | Data and the rules for changing it: a path, a pose, a map, the agent model. | Only math types such as `Vector2`. |
| **Application** | *Who does what, and when?* | Services that orchestrate the domain: the bake chain, the search, the planning loop, the passes. | No components, no assets. |
| **Presentation** | *How does this appear in Unity?* | Components, ScriptableObjects, Inspector fields, gizmos, and wiring. | Yes. |

Dependencies only point **downward**: presentation uses application, application uses domain, and never the other way around. A quick test for where something belongs:

- It needs a MonoBehaviour, a ScriptableObject or the Editor → **Presentation**.
- It decides an order, a timing, or who calls whom → **Application**.
- It is true regardless of any scenario and could be tested without Unity → **Domain**.

One feature is often spread over all three. Swapping a new plan in while the agent is moving is a good example: the *rule* that the cursor and speed survive a swap lives on [PathAgentModel](Domain/Agent/PathAgentModel.md) (domain); the *decision* to swap hot instead of starting cold lives in [PathAgentPipeline](Pipeline/Application/Agent/PathAgentPipeline.md) (application); and [PathPlanner](Pipeline/Presentation/PathPlanner.md) (presentation) just calls it.

## How the assemblies line up

Putting the halves and layers together gives the assemblies. Each has a matching namespace.

```
                               NCPF.Domain
                   ┌─────────────┘   ▲   └─────────────┐
                   │                 │                 │
       NCPF.Bake.Application         │        NCPF.Pipeline.Application
                   ▲                 │                 ▲
                   │                 │                 │
       NCPF.Bake.Presentation ──► NCPF.Shared.Presentation ◄── NCPF.Pipeline.Presentation
                   ▲                 ▲                 ▲
       NCPF.Bake.Editor         NCPF.Config         NCPF.Pipeline.Editor
```

| Assembly | Holds |
|---|---|
| `NCPF.Domain` | Paths, poses, the map, the control set, the agent model. |
| `NCPF.Shared.Presentation` | The storage ports and the scene grid that both halves use. |
| `NCPF.Config` | The concrete assets you create in the Project window. |
| `NCPF.Bake.Application` / `.Presentation` / `.Editor` | The bake: services, the baking components, their Inspector tools. |
| `NCPF.Pipeline.Application` / `.Presentation` / `.Editor` | The runtime: search and driving, the planner component, its Inspector tools. |

The compiler enforces the split: the bake assemblies cannot see the pipeline assemblies and the other way around, and neither application assembly can see its own half's presentation assembly. `NCPF.Shared.Presentation` is the exception both halves may use.

## Ports: why nothing references Config

Baked data has to be stored somewhere, but the code that uses it should not care where. NCPF separates the two with **ports**.

A port is an abstract ScriptableObject in `NCPF.Shared.Presentation` — [MapAsset](Shared/MapAsset.md), [ControlSetAsset](Shared/ControlSetAsset.md), [AgentConfig](Shared/AgentConfig.md), [NcpfSettingsAsset](Shared/NcpfSettingsAsset.md). Every field that needs baked data is typed as a port.

The concrete assets — [MapContainer](Config/MapContainer.md), [FuncControlSetContainer](Config/FuncControlSetContainer.md), [NCPFSettings](Config/NCPFSettings.md) — live in `NCPF.Config`, and **no assembly references `NCPF.Config`**. When you drag a MapContainer into a field typed `MapAsset`, Unity binds the concrete asset for you, and every `Read` or `Write` call reaches the concrete implementation through the port.

The payoff: to store maps in a different format — JSON, a binary file, a database — you add one new class to `NCPF.Config` and nothing else in the package changes or recompiles.

## What happens in one frame

Here is the runtime path of a single agent, from the component down.

**Wiring (once).** In `Start`, [PathPlanner](Pipeline/Presentation/PathPlanner.md) asks [NcpfGraphComposer](Pipeline/Presentation/NcpfGraphComposer.md) to build its dependencies from the Inspector fields: the map, the control set, the search graph, and the finders. It wires them into a [PathAgentPipeline](Pipeline/Application/Agent/PathAgentPipeline.md) (the planning loop) and a [PathFollowAgent](Pipeline/Application/Agent/PathFollowAgent.md) (the driver). If something outside already injected a context through `Construct`, this step is skipped.

**Update — planning.** Every frame, [PathAgentPipeline](Pipeline/Application/Agent/PathAgentPipeline.md) decides whether a new plan is needed:

1. If the current plan still ends near the target, nothing happens.
2. If a search is already running, it waits.
3. If a finished search result is waiting, it stitches the new path onto the one the agent is driving and hands it to the follower. The agent keeps its speed and heading — there is no jump.
4. Otherwise, if enough time has passed since the last search, it starts a new one.

The search itself runs on a background thread, so planning never stalls the frame.

**FixedUpdate — driving.** [PathFollowAgent](Pipeline/Application/Agent/PathFollowAgent.md) advances the agent along the plan. It asks a chain of **passes** what to do: the main pass decides the speed and facing, optional passes adjust them, and only then does the follower move. PathPlanner writes the result to the Transform.

## The search

A position in the lattice — a **node** — is a cell plus a heading, packed into an integer id. An **edge** is one primitive curve from the control set, placed at that node.

The search graph is a stack of decorators. Each layer does one job:

```
CachedCurvateGraph                remembers each node's edges after the first time
 └─ RearGearGraph                 makes reversing more expensive
     └─ CurvateMap3D              the lattice itself
         └─ PassabilityTransitionGenerator   emits the edges that fit, with their cost
```

A few design decisions make this fast enough to run for many agents:

- **Collision is precomputed.** A primitive's shape is the same at every cell — only its position changes. What differs per cell is whether it fits. [PassabilityBaker](Pipeline/Application/Space/PassabilityBaker.md) checks that once for every node and stores it as one bit per primitive in a [PrimitivePassability](Domain/Space/PrimitivePassability.md) mask. The search reads a bit instead of testing cells.
- **Edges carry no geometry.** A\* only needs *where an edge leads* and *what it costs*. A [TransitionData](Domain/Space/TransitionData.md) holds exactly that, plus the primitive's id. The actual curves are rebuilt only for the winning path, by [PlanGeometry](Pipeline/Application/Search/PlanGeometry.md).
- **Costs are precomputed per primitive.** An edge's cost depends on the primitive's length and curvature, not on where it is placed, so it is computed once when the graph is built.
- **Cost is time.** A curve costs the time it takes to drive it at the highest speed its tightest point allows. Tighter curves force slower speeds and therefore cost more, so the search naturally prefers smooth paths.

[EpsilonLadderFinder](Pipeline/Application/Search/EpsilonLadderFinder.md) runs the search as a ladder of heuristic weights. A weight above 1 makes the search greedier: it finds a path faster, but not necessarily the best one. The rungs are tried in the order you configure, and the first rung that finds a path within the iteration budget wins.

## Driving: the model and the passes

The agent's state lives in exactly one place, [PathAgentModel](Domain/Agent/PathAgentModel.md): the path, how far along it the agent is, and its current speed and pose. Nothing else keeps a copy.

Decisions come from **passes** ([SAPass](Pipeline/Application/Agent/SAPass.md)). A pass looks at the state and returns a control input — a target speed and a target facing. It never moves the agent itself. The follower collects every pass's decision in order and moves the agent once, at the end, so the last pass always has the final word.

The agent is **holonomic in facing**: it can look one way and travel another. Speed is **signed** — positive when driving nose-first, negative when reversing — so a change of direction always passes through a full stop.

## Features: authoring behaviour in the Inspector

Passes and goal logic are application code, but you configure them in the Inspector through **features** — small serializable classes you pick from a dropdown on [PathPlanner](Pipeline/Presentation/PathPlanner.md). There are four kinds: the main planner, an achievement (when is the target reached), a heuristic (how far is it), and optional overrides. See [Planner features](Pipeline/Presentation/PlannerFeatures.md).

A feature only *creates* the pass or builder. [BehaviorBus](Pipeline/Application/Agent/BehaviorBus.md) holds the live ones behind swappable slots, so you can change a feature in Play mode and the running search and follower pick it up without restarting.

## Dependencies and injection

Objects in NCPF do not create their own dependencies. The rule is:

- **Presentation creates and wires.** Components such as [PathPlanner](Pipeline/Presentation/PathPlanner.md) and [ControlSetBaker](Bake/Presentation/ControlSetBaker.md) decide which concrete classes are used and pass them down.
- **Application and domain receive.** Services take everything they need through their constructor.

Each presenter has a standard setup it builds on its own, and a way to accept a setup from outside — `PathPlanner.Construct` and `ControlSetBaker.Construct`. That is the hook for plugging NCPF into your own dependency injection.

### Sharing between agents

Many agents often use the same map and control set. Building those per agent would repeat the most expensive work — the passability bake in particular — once for every agent.

In Play mode, [NcpfPlannerBootstrap](Pipeline/Presentation/NcpfPlannerBootstrap.md) runs before any planner builds and hands every planner to [MulPlannerDependencyResolver](Pipeline/Presentation/MulPlannerDependencyResolver.md). It groups planners from general to specific, building each dependency once per group:

| Level | Grouped by | Shared |
|---|---|---|
| Map | map asset | the lattice map |
| Control set | + control set asset | the discretized control set, the passability mask, the plan geometry |
| Agent | + dynamics and preferences | the search graph and its cache |
| Planner | — | the agent model, the target, the finders |

Agents that share a graph also share its cache, so a node explored by one agent is free for all the others.

## Threading

Searches run on thread-pool threads, and several can run at once. This is safe because everything the search reads is **immutable once built**: the map, the control set, the passability mask and the cost tables are never written after construction. The one shared object that does change, the neighbour cache in [CachedCurvateGraph](Pipeline/Application/Space/CachedCurvateGraph.md), uses concurrent collections.

The one exception is the scene grid: heuristics convert cells to world positions through the [Unity2DGrid](Shared/Unity2DGrid.md) component. Keep its **Draw Gizmos** off in Play mode, because drawing re-reads the colliders while searches may be reading the grid.

The rule for extending: if you add something the search reads, build it completely before the first search and never change it afterwards.

## Where to go next

- [Getting Started](GettingStarted.md) — set up an agent.
- [Documentation home](README.md) — every page, grouped by assembly.
- [FAQ](FAQ.md) — common problems.
