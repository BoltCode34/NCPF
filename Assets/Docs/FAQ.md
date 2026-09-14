# FAQ

Common problems and how to fix them. If your problem is not here, check that you followed [Getting Started](GettingStarted.md) in order.

## Setup and baking

### I get a DivideByZeroException

The map has not been baked, so it has no headings, and converting a pose to a lattice node divides by zero.

Select the object with [NCPFMapBuilder](Bake/Presentation/NCPFMapBuilder.md) (in the sample scenes, **NCPFMap**) and run **Build** from its context menu. See [MapContainer](Config/MapContainer.md).

### The agent never moves

Check, in order:

1. **The planner is wired.** [PathPlanner](Pipeline/Presentation/PathPlanner.md) builds nothing unless **Grid**, **Map Container** and **Control Set Container** are all set.
2. **The map is baked** for the current grid. See the question above.
3. **The control set is baked** with **Bake**, not just **Bake Layer**. **Bake Layer** overwrites the asset with a single heading. The control set lives only in its asset: a container that was never baked is empty, even if another container holds a set. See [ControlSetBaker](Bake/Presentation/ControlSetBaker.md).
4. **The agent starts in a free pose.** The search returns nothing if the agent's own pose is blocked, for example when its body overlaps a wall at its current heading. Move it into open space.
5. **There is a path within budget.** See the next question.

### No path is found, and there is no error

Search errors are not reported: an exception in the background search looks exactly like "no path". Possible causes:

- **The budget is too small.** Raise **Attempt Budget** on the PathPlanner, or add a more careful rung to **Epsilon Ladder**. See [EpsilonLadderFinder](Pipeline/Application/Search/EpsilonLadderFinder.md).
- **The control set cannot reach the target cell.** Curves only end on certain cells. Use [RingAchivementFeature](Pipeline/Application/Search/RingAchivementBuilder.md) with a ring a few cells wide, or bake with a larger **Max Radius**.
- **The gap is too narrow for the curves.** The body may fit, but no curve of the set fits through. Lower **Min Turn Radius** or the cell size and bake again.
- **The data does not match.** A control set baked for a different cell size or heading count, or a map baked for a different grid. Bake both again.

To see an exception that was swallowed, set a breakpoint in `PathFindingAgent.BuildPathAsync`.

### The control set asset is only partly filled

[FuncControlSetContainer](Config/FuncControlSetContainer.md) stores only polynomial curves, such as clothoids. It clears the asset first, and stops with an error at the first curve it cannot store, leaving the rest unsaved. Use an [IPathBuilder](Bake/Application/IPathBuilder.md) that produces `IPolynomialPath` curves.

### Baking the control set takes very long

Every ring of cells up to **Max Radius** is baked; there is no early stop when rings stop contributing curves. Lower **Max Radius** to the distance where the preview shows no new curves. See [LayerControlSetBuilderBase](Bake/Application/LayerControlSetBuilderBase.md).

### Rotated curves end in the wrong place

The map's heading count must be a multiple of 4. Only a quarter of the headings is baked and the rest is rotated, which only works in whole heading steps. See [QuarterSymmetry](Domain/ControlSet/QuarterSymmetry.md).

## Shape and collisions

### The agent clips through corners

- **The anchor does not match the pivot.** The planner moves the transform's position along the path, and the map is baked around the shape's **Anchor**. They must be the same point of the body. Check it in the shape's Inspector preview, see [AgentConfig](Shared/AgentConfig.md#inspector-preview).
- **The shape changed but the map was not rebaked.** The map does not record which shape it is for, and nothing checks it: a planner with a map baked for another shape uses it silently. Bake the map again after changing the shape.
- **The cell size is too coarse.** Obstacles are detected per cell. Thin walls and narrow bodies need smaller cells.

### My body is not a rectangle

Use [CircleConfig](Config/CircleConfig.md), [CapsuleConfig](Config/CapsuleConfig.md) or [TriangleConfig](Config/TriangleConfig.md), or assemble the body from several shapes in a [CompositeConfig](Config/CompositeConfig.md). A rectangle that covers an I-beam or an L-shaped body refuses gaps the real body fits through. Bake the map again after switching.

### The agent drives outside the grid near its edges

The part of the body that hangs past the grid's edge is not checked when baking the map. Surround the area with obstacles. See [MapBuilder](Domain/Collision/MapBuilder.md).

## Several agents

### Agents don't share their map and graph

- **Sharing works only in Play mode.** In Edit mode, each planner builds its own copy.
- **Sharing is turned off** in the `NCPFSettings` asset. See [NCPFSettings](Config/NCPFSettings.md).
- **The planners differ.** Only planners with the same grid, containers and preferences share. See [MulPlannerDependencyResolver](Pipeline/Presentation/MulPlannerDependencyResolver.md).
- **The planner was created after the scene loaded.** Call `NcpfPlannerBootstrap.Resolve` for it. See [NcpfPlannerBootstrap](Pipeline/Presentation/NcpfPlannerBootstrap.md).

### Sharing stopped after I changed a planner in the Inspector

A planner whose dependencies or preferences change rebuilds its own copy and leaves its group. Restart Play mode to regroup.

## Performance

### The Editor is slow while the scene is open

Turn off **Draw Gizmos** on the [Unity2DGrid](Shared/Unity2DGrid.md). It re-reads all obstacles every time the Scene view redraws.

### Entering Play mode or changing a planner causes a freeze

When a planner builds its dependencies, it bakes the passability data for the whole map: every curve is checked at every pose. It is done once per map and control set, and shared between agents in Play mode. Changing a planner's Inspector rebuilds it. See [PassabilityBaker](Pipeline/Application/Space/PassabilityBaker.md).

### Searches get slower in cluttered levels

The default heuristic ignores walls and sends the search into dead ends. Switch the heuristic to [DijkstraHeuristicFeature](Pipeline/Application/Search/DijkstraHeuristicBuilder.md).
