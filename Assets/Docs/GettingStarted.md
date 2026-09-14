# Getting Started

This guide takes you from an empty level to an agent that plans a path and drives toward a target. It takes about ten minutes.

NCPF is developed on Unity 6.2. If you want to see a finished setup first, open one of the sample scenes:

- `Assets/Materials/Scenes/SingleAgent.unity` — one agent chasing a target.
- `Assets/Materials/Scenes/MultipleAgent.unity` — a group of agents sharing one map and one control set.
- `Assets/Materials/Scenes/IBeamAgent.unity` — an I-beam-shaped agent finding its way through narrow gaps.

Their assets are in `Assets/Materials/Configs`: `Rectangle` for the first two scenes, `IBeam` for the third.

## Overview

Setting up NCPF has two phases.

**In the Editor, once per level and agent shape**, you bake two assets:

| Asset | What it holds | Baked by |
|---|---|---|
| Map | For every cell and every heading: does the agent's body fit here? | [NCPFMapBuilder](Bake/Presentation/NCPFMapBuilder.md) |
| Control set | The library of short curves the agent can drive. | [ControlSetBaker](Bake/Presentation/ControlSetBaker.md) |

**At runtime**, a [PathPlanner](Pipeline/Presentation/PathPlanner.md) on each agent uses those two assets to plan and drive.

The order matters. The map depends on the agent's shape, and the control set is baked against the map's grid, so you set things up in this order:

1. Obstacles and grid
2. Agent shape
3. Map
4. Control set
5. Agent dynamics
6. Planner

## 1. Prepare the obstacles and the grid

NCPF finds obstacles with 2D physics. Every obstacle in your level needs a **Collider2D**, and must be on a layer you reserve for obstacles.

Then add a grid that samples those colliders:

1. Create an empty GameObject and name it `Grid`.
2. Add the [Unity2DGrid](Shared/Unity2DGrid.md) component.
3. Place the GameObject at the **lower-left corner** of the area the agents move in. The grid's first cell sits at the GameObject's position and the grid extends along +X and +Y.
4. Set the fields:

| Field | What to set |
|---|---|
| **World Size** | Width and height of the area, in metres. |
| **Cell Size** | Size of one cell, in metres. The map describes the body as a set of cells, so this decides how precisely the shape is represented: smaller cells are more precise, but the map gets larger and slower to bake. For reference, the sample scenes use 0.3 m cells in a 20 × 10 m area. |
| **Obstacle Mask** | The layer (or layers) your obstacle colliders are on. |
| **Bake On Awake** | Leave enabled. |
| **Draw Gizmos** | Enable to check the grid in the Scene view: free cells are cyan, blocked cells are red. |

> **Tip:** Draw Gizmos re-reads the colliders on every redraw of the Scene view. Use it to check your setup, then turn it off.

## 2. Describe the agent's shape

The map is computed for one specific body shape, so the shape comes first.

Pick the config that matches your body:

| Config | Use for |
|---|---|
| [RectangularConfig](Config/RectangularConfig.md) | Boxes, needles, vehicles. |
| [CircleConfig](Config/CircleConfig.md) | Round bodies. |
| [CapsuleConfig](Config/CapsuleConfig.md) | Elongated bodies with rounded ends. |
| [TriangleConfig](Config/TriangleConfig.md) | Arrow-like bodies. |
| [CompositeConfig](Config/CompositeConfig.md) | Anything else, assembled from the shapes above, such as an I-beam. |

This guide uses a rectangle:

1. In the Project window, select **Create > NCPF > RectangularConfig**.
2. Set the fields:

| Field | What to set |
|---|---|
| **Size** | Width and length of the body, in metres. The sample agent is 0.16 × 2 m. |
| **Anchor** | The point the body is moved by, as a fraction of the rectangle: `(0.5, 0.5)` is the centre, `(0.5, 1)` is the middle of the front edge. This must match the pivot of your agent's GameObject. |

The Inspector shows a preview of the body as a grid of cells, with the anchor cell highlighted. Set **Preview Cell Size** to your grid's cell size to see exactly what will be baked. See [AgentConfig](Shared/AgentConfig.md#inspector-preview).

> **Important:** The anchor is baked into the map. If you change it, bake the map again.

## 3. Bake the map

1. In the Project window, select **Create > NCPF > Containers > MapContainer**. This asset stores the baked map.
2. Create an empty GameObject, name it `Map`, and add the [NCPFMapBuilder](Bake/Presentation/NCPFMapBuilder.md) component.
3. Set the fields:

| Field | What to set |
|---|---|
| **Map** | The MapContainer asset you created. |
| **Agent** | The RectangularConfig asset from step 2. |
| **Grid** | The `Grid` GameObject. |
| **Angle Step Count** | How many headings the map distinguishes. **Must be a multiple of 4.** 16 (steps of 22.5°) is a good default. |

4. Right-click the NCPFMapBuilder component header and select **Build**.

Select the MapContainer asset to check the result: the Inspector shows the map one heading at a time, with a slider to switch between them. White cells are where the body fits at that heading.

> **Note:** Bake the map again whenever you move obstacles, change the grid, or change the agent shape.

## 4. Bake the control set

1. In the Project window, select **Create > NCPF > Containers > FuncControlSetContainer**. This asset stores the curve library.
2. Create an empty GameObject, name it `ControlSet`, and add the [ControlSetBaker](Bake/Presentation/ControlSetBaker.md) component.
3. Set the references:

| Field | What to set |
|---|---|
| **Map Container** | The baked MapContainer. |
| **Control Set Container** | The FuncControlSetContainer asset you created. |
| **Grid** | The `Grid` GameObject. |

4. Right-click the component header and select **Bake**.

The remaining fields control which curves end up in the library. The defaults work; change them once you see how your agent drives:

| Field | Effect |
|---|---|
| **Max Radius** | How far from the start the baker generates curves, in metres. The bake always works out to this distance, so it sets the longest possible curve and most of the bake time. Raise it if long curves are missing. |
| **Min Turn Radius** | Rejects curves that turn tighter than this, in metres. `0` means no limit. Set it to the tightest turn your agent can make. |
| **Position Tube** / **Angle Tube** | How close a curve must pass to a shorter curve's end, in metres and degrees, to count as made of shorter curves and be dropped. Larger values give a smaller, coarser library; smaller values keep more curves. |
| **Length Multiplier** | Rejects curves longer than this many times the straight distance between their ends. Keeps wandering curves out. |

To look at the result, enable **Draw**, pick a heading with **Angle Layer**, and look at the Scene view. **Bake Layer** in the context menu bakes only that one heading, which is faster while you tune the values.

## 5. Describe the agent's dynamics

1. In the Project window, select **Create > NCPF > Configs > DynamicAgentConfig**.
2. Set the agent's limits:

| Field | Meaning |
|---|---|
| **Max Velocity** | Top speed, in metres per second. |
| **Max Linear Acceleration** | How fast the agent speeds up and brakes, in metres per second squared. |
| **Max Angled Velocity** | How fast the agent can turn its body, in degrees per second. |

See [DynamicAgentConfig](Pipeline/Presentation/DynamicAgentConfig.md).

## 6. Add the planner to the agent

1. Select your agent's GameObject.
2. Add the [PathPlanner](Pipeline/Presentation/PathPlanner.md) component.
3. Fill in the **Dependencies**:

| Field | What to set |
|---|---|
| **Grid** | The `Grid` GameObject. |
| **Map Container** | The baked MapContainer. |
| **Control Set Container** | The baked FuncControlSetContainer. |
| **Dynamic Agent** | The DynamicAgentConfig. |
| **Target** | Drag in the Transform the agent should chase. |
| **Agent Shape** | The RectangularConfig. |

The agent does not need a Rigidbody. PathPlanner moves the Transform directly.

Leave everything else at its defaults for now. The **Features** slots are empty on purpose: an empty slot uses the standard behaviour — reach the target's cell, estimate the distance in a straight line, and drive with a searched speed profile. See [PlannerStandards](Pipeline/Presentation/PlannerStandards.md).

## 7. Press Play

The agent searches for a path, starts driving, and replans as the target moves.

Enable **Draw** on the PathPlanner to see the plan in the Scene view. PathPlanner also runs in Edit mode, so you can check a plan without entering Play mode.

## Next steps

### Tune how the agent drives

- **NCPF Preferences** on the PathPlanner let you discourage reversing (**Rear Gear Penalty**) and tight turns (**Curvature Penalty**).
- The **Features** slots let you replace the standard behaviour — for example, stop at a distance from the target with a ring achievement, or look at the target while driving. See [Planner features](Pipeline/Presentation/PlannerFeatures.md).

### Use many agents

Agents that use the same map, control set, dynamics and preferences share one copy of everything they can share, including the search graph and its cache. This happens automatically in Play mode; see [NcpfPlannerBootstrap](Pipeline/Presentation/NcpfPlannerBootstrap.md).

To switch it off, create **Create > NCPF > Settings**, name the asset `NCPFSettings`, place it in a `Resources` folder, and disable **Combine Dependencies**. See [NCPFSettings](Config/NCPFSettings.md).

If you spawn agents at runtime, call `NcpfPlannerBootstrap.Resolve(planner)` after adding the PathPlanner so the new agent joins the others.

### Understand the package

- [Architecture](Architecture.md) — how the package is organised and why.
- [Documentation home](README.md) — every page, grouped the way the code is.
- [FAQ](FAQ.md) — common problems and how to fix them.
