# NCPF Documentation

**NCPF — Non-Circular Path Finder** plans paths for agents that cannot turn on the spot and whose body shape matters: long, narrow or asymmetric bodies, where the orientation decides whether the agent fits through a gap.

New here? Go to **[Getting Started](GettingStarted.md)** — it walks you from an empty scene to an agent driving toward a target.

## Why NCPF exists

Most pathfinding treats the agent as a point or a circle and plans on a grid where every cell connects to its neighbours. That breaks down for a real vehicle-like body:

- **A circle lies about the shape.** Make it big enough to cover the body and it refuses gaps the body actually fits through sideways. Make it smaller and the corners of the body clip walls.
- **A grid path is not drivable.** A path that turns 90° in one cell has to be smoothed afterwards, and smoothing cannot always find a curve that the body can follow without hitting something.

NCPF plans differently. Before the game runs, it computes a small library of short curves the agent can really drive — the **control set** — and it computes, for every cell and every heading, whether the real body shape fits there — the **map**. At runtime the search only ever strings those curves together, so every path it returns is already drivable and already collision-free for the actual shape. Nothing is smoothed afterwards.

This is the *state lattice* approach from robotics research. The [Architecture](Architecture.md) page explains it in more depth.

## How it works, in four steps

1. **Bake the map.** [NCPFMapBuilder](Bake/Presentation/NCPFMapBuilder.md) reads your level's obstacles through a [Unity2DGrid](Shared/Unity2DGrid.md) and, for every heading, marks where the agent's footprint fits.
2. **Bake the control set.** [ControlSetBaker](Bake/Presentation/ControlSetBaker.md) generates the library of drivable primitive curves.
3. **Plan.** [PathPlanner](Pipeline/Presentation/PathPlanner.md) searches for a chain of primitives from the agent to its target.
4. **Follow.** The agent drives along that chain, choosing its speed and facing as it goes, and replans when the target moves.

Steps 1 and 2 happen in the Editor, once per level and per agent shape. Steps 3 and 4 happen every frame in Play mode.

## Where to start reading

These are the objects the rest of the package is built around. Reading them first makes everything else easier to place.

| Object | Why start here |
|---|---|
| [PathPlanner](Pipeline/Presentation/PathPlanner.md) | The component you put on an agent. Everything at runtime starts from it. |
| [NCPFMapBuilder](Bake/Presentation/NCPFMapBuilder.md) | Bakes the map. The first thing you run on a new level. |
| [ControlSetBaker](Bake/Presentation/ControlSetBaker.md) | Bakes the primitive library. The second thing you run. |
| [PathAgentPipeline](Pipeline/Application/Agent/PathAgentPipeline.md) | The planning loop: when to search, when to replan, how a new plan replaces the old one. |
| [PathFollowAgent](Pipeline/Application/Agent/PathFollowAgent.md) | Drives the body along the plan and decides speed and facing through passes. |
| [ControlSet](Domain/ControlSet/ControlSet.md) | The primitive library itself. |
| [IPath](Domain/Core/IPath.md) | What a primitive is: pure geometry, a pose as a function of distance. |

## Documentation map

The documentation is grouped the same way as the code. The [Architecture](Architecture.md) page explains why it is split this way.

### Domain

Pure data and the rules for changing it. No Unity components, no scene objects.

- **Core** — [IPath](Domain/Core/IPath.md), [WorldConfig](Domain/Core/WorldConfig.md), [Config](Domain/Core/Config.md), [DynamicState](Domain/Core/DynamicState.md), [DynamicAgent](Domain/Core/DynamicAgent.md), [AngleUtility](Domain/Core/AngleUtility.md), [IGraph](Domain/Core/IGraph.md)
- **Paths** — [ClothoidPath](Domain/Paths/ClothoidPath.md), [RearFirstPath](Domain/Paths/RearFirstPath.md), [FunctionBasedPath](Domain/Paths/FunctionBasedPath.md), [BezierPath](Domain/Paths/BezierPath.md), [DiscretizedPath3D](Domain/ControlSet/DiscretizedControlSet3D.md)
- **Collision** — [Map3D](Domain/Collision/Map3D.md), [MapBuilder](Domain/Collision/MapBuilder.md), [Agent](Domain/Collision/Agent.md) (Rectangle, Circle, Capsule, Triangle), [CompositeAgent](Domain/Collision/CompositeAgent.md)
- **Control set** — [ControlSet](Domain/ControlSet/ControlSet.md), [DiscretizedControlSet3D](Domain/ControlSet/DiscretizedControlSet3D.md), [QuarterSymmetry](Domain/ControlSet/QuarterSymmetry.md)
- **Space** — [IGridMap3D](Domain/Space/IGridMap3D.md), [PrimitivePassability](Domain/Space/PrimitivePassability.md), [TransitionData](Domain/Space/TransitionData.md)
- **Agent** — [PathAgentModel](Domain/Agent/PathAgentModel.md), [PrimitiveWindow](Domain/Agent/PrimitiveWindow.md), [CurvatureSpeedProfile](Domain/Agent/CurvatureSpeedProfile.md), [PrimitiveAnalyzer](Domain/Agent/PrimitiveAnalyzer.md), [PathGearUtility](Domain/Agent/PathGearUtility.md), [PathLimiterBase](Domain/Agent/PathLimiterBase.md)

### Shared

The storage ports, the scene grid and the Inspector type picker that both halves of the package use.

[MapAsset](Shared/MapAsset.md), [ControlSetAsset](Shared/ControlSetAsset.md), [AgentConfig](Shared/AgentConfig.md), [NcpfSettingsAsset](Shared/NcpfSettingsAsset.md), [SpacedMap3D](Shared/SpacedMap3D.md), [Unity2DGrid](Shared/Unity2DGrid.md), [Shared editors](Shared/Editor/SharedEditors.md)

### Config

The concrete assets you create in the Project window.

- **Containers and settings** — [MapContainer](Config/MapContainer.md), [FuncControlSetContainer](Config/FuncControlSetContainer.md), [NCPFSettings](Config/NCPFSettings.md)
- **Body shapes** — [RectangularConfig](Config/RectangularConfig.md), [CircleConfig](Config/CircleConfig.md), [CapsuleConfig](Config/CapsuleConfig.md), [TriangleConfig](Config/TriangleConfig.md), [CompositeConfig](Config/CompositeConfig.md)

### Bake

Everything that runs in the Editor to produce the map and the control set.

- **Application** — [ControlSetBakeService](Bake/Application/ControlSetBakeService.md), [ControlSetBuilder](Bake/Application/ControlSetBuilder.md), [LayerControlSetBuilderBase](Bake/Application/LayerControlSetBuilderBase.md), [BackwardLayerBuilder](Bake/Application/BackwardLayerBuilder.md), [IPathBuilder](Bake/Application/IPathBuilder.md), [Bake logging](Bake/Application/BakeLogging.md)
- **Presentation** — [NCPFMapBuilder](Bake/Presentation/NCPFMapBuilder.md), [ControlSetBaker](Bake/Presentation/ControlSetBaker.md), [ControlSetBuildLogger](Bake/Presentation/ControlSetBuildLogger.md)
- **Editor** — [Bake editors](Bake/Editor/BakeEditors.md)

### Pipeline

Everything that runs in Play mode to plan and drive.

- **Agent** — [PathAgentPipeline](Pipeline/Application/Agent/PathAgentPipeline.md), [PathFollowAgent](Pipeline/Application/Agent/PathFollowAgent.md), [BehaviorBus](Pipeline/Application/Agent/BehaviorBus.md), [PathAgentContext](Pipeline/Application/Agent/PathAgentContext.md), [SAPass](Pipeline/Application/Agent/SAPass.md), [LookPass](Pipeline/Application/Agent/LookPass.md), [MotionWindow](Pipeline/Application/Agent/MotionWindow.md), [ITarget](Pipeline/Application/Agent/ITarget.md), [PlanRollout](Pipeline/Application/Agent/PlanRollout.md)
- **Search** — [PathFindingAgent](Pipeline/Application/Search/PathFindingAgent.md), [EpsilonLadderFinder](Pipeline/Application/Search/EpsilonLadderFinder.md), [GoalPathFinderBase](Pipeline/Application/Search/GoalPathFinderBase.md), [IAsyncGoalPathFinder](Pipeline/Application/Search/IAsyncGoalPathFinder.md), [IGoal](Pipeline/Application/Search/IGoal.md), [IGoalBuilder](Pipeline/Application/Search/IGoalBuilder.md), [PlanGeometry](Pipeline/Application/Search/PlanGeometry.md), [DiscretizedControlSet3DConverter](Pipeline/Application/Search/DiscretizedControlSet3DConverter.md), [DijkstraHeuristicBuilder](Pipeline/Application/Search/DijkstraHeuristicBuilder.md), [DirectDistHeuristicsBuilder](Pipeline/Application/Search/DirectDistHeuristicsBuilder.md), [PointCellAchivementBuilder](Pipeline/Application/Search/PointCellAchivementBuilder.md), [RingAchivementBuilder](Pipeline/Application/Search/RingAchivementBuilder.md)
- **Space** — [ICurvateGraph](Pipeline/Application/Space/ICurvateGraph.md), [CurvateMap3D](Pipeline/Application/Space/CurvateMap3D.md), [ITransitionGenerator](Pipeline/Application/Space/ITransitionGenerator.md), [PassabilityTransitionGenerator](Pipeline/Application/Space/PassabilityTransitionGenerator.md), [PassabilityBaker](Pipeline/Application/Space/PassabilityBaker.md), [CachedCurvateGraph](Pipeline/Application/Space/CachedCurvateGraph.md), [RearGearGraph](Pipeline/Application/Space/RearGearGraph.md), [PathOffset](Pipeline/Application/Space/PathOffset.md), [SpacedMapPV](Pipeline/Application/Space/SpacedMapPV.md), [FacingModeMap](Pipeline/Application/Space/FacingModeMap.md), [IPrimitivesProjector3D](Pipeline/Application/Space/IPrimitivesProjector3D.md)
- **Presentation** — [PathPlanner](Pipeline/Presentation/PathPlanner.md), [NcpfGraphComposer](Pipeline/Presentation/NcpfGraphComposer.md), [NcpfPlannerBootstrap](Pipeline/Presentation/NcpfPlannerBootstrap.md), [MulPlannerDependencyResolver](Pipeline/Presentation/MulPlannerDependencyResolver.md), [PathFeatureContainer](Pipeline/Presentation/PathFeatureContainer.md), [PlannerStandards](Pipeline/Presentation/PlannerStandards.md), [Planner features](Pipeline/Presentation/PlannerFeatures.md), [TravelPlannerFeature](Pipeline/Presentation/TravelPlannerFeature.md), [LookAtTargetFeature](Pipeline/Presentation/LookAtTargetFeature.md), [DynamicAgentConfig](Pipeline/Presentation/DynamicAgentConfig.md), [DynamicsVisualizer](Pipeline/Presentation/DynamicsVisualizer.md)

## Changing the behaviour

You rarely need to touch the search itself. These are the intended places to plug in your own logic:

| You want to… | Extend |
|---|---|
| Change how the agent chooses its speed and facing | A [MainPlannerFeature](Pipeline/Presentation/PlannerFeatures.md) |
| Add a behaviour on top, such as looking at something while driving | A [PlannerOverrideFeature](Pipeline/Presentation/PlannerFeatures.md) with an [SAPass](Pipeline/Application/Agent/SAPass.md) |
| Change what counts as reaching the target | An [AchivementFeature](Pipeline/Presentation/PlannerFeatures.md) |
| Change how the search estimates the remaining distance | A [HeuristicFeature](Pipeline/Presentation/PlannerFeatures.md) |
| Make some moves cost more, such as avoiding an area | A decorator over [ICurvateGraph](Pipeline/Application/Space/ICurvateGraph.md), like [RearGearGraph](Pipeline/Application/Space/RearGearGraph.md) |
| Use a different agent shape | A [CompositeConfig](Config/CompositeConfig.md) assembled from existing shapes, or your own [Agent](Domain/Collision/Agent.md) and [AgentConfig](Shared/AgentConfig.md) |
| Use a different family of curves | An [IPathBuilder](Bake/Application/IPathBuilder.md) |
| Store baked data somewhere else | A [MapAsset](Shared/MapAsset.md) or [ControlSetAsset](Shared/ControlSetAsset.md) |

## Something went wrong?

See the [FAQ](FAQ.md) for the common problems and how to fix them.

## Writing documentation

New pages follow the [page standard](PageStandard.md).
