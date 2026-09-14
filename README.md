NCPF — Non-Circular Path Finder

NCPF is a geometric path planner for agents that are non-holonomic (limited turning radius) and whose body shape can't be safely approximated by a bounding circle — drone-like or elongated bodies, anything where orientation actually changes what fits through a gap.

Instead of planning on a raw occupancy grid and fixing up curvature afterward, NCPF bakes a control set of feasible motion primitives once per configuration cell, assembles it into a curvature-aware search graph, and plans directly over that graph. Every edge the planner can take is a path the agent can actually drive — there's no post-processing step trying to make a jagged grid path drivable.

Why this approach

Planning with holonomic assumptions (4- or 8-connected grids) and smoothing the result afterward tends to break down for vehicles with real turning constraints and non-circular footprints: the smoothing step can't always recover a feasible path, and collision checks against a circular proxy either miss corners or reject valid maneuvers.

NCPF instead builds what's known in the literature as a state lattice: a discretized configuration space where connectivity is defined only by paths the vehicle can actually execute. A control set — a small, near-minimal set of motion primitives repeated at every node — keeps the branching factor of the search low while still preserving the ability to reconstruct any feasible path in the lattice through concatenation. This is the same idea explored in:

Pivtoraiko & Kelly, Generating Near-Minimal Spanning Control Sets for Constrained Motion Planning in Discrete State Spaces
Divelbiss & Wen, Nonholonomic Path Planning with Inequality Constraints

NCPF adapts and implements this for arbitrary, non-circular agent shapes on a 3D (x, y, heading) configuration grid.

How it works, in short
Bake — for a given agent shape and grid resolution, generate a control set: the minimal set of curvature-feasible primitives reachable from a cell, pruned by ring-growth and path decomposition so redundant (decomposable) primitives are dropped.
Discretize & validate — each primitive is discretized into the grid cells it sweeps, so collision checking against the agent's real footprint is exact, not circle-approximated.
Graph — the baked control set becomes a curvature-aware graph: nodes are (x, y, heading) cells, edges are the primitives, weighted by path cost.
Plan & follow — a signed-speed search runs over that graph to produce a plan, and a dedicated follower agent tracks it, handling replanning and hot/cold plan swaps as the target moves.
Demos

Control set

The baked library of primitives from one cell: every curve ends exactly on a lattice node, and none of them can be assembled from shorter ones.

IMAGE_CONTROL_SET

Single agent

One agent replanning toward a moving target through a cluttered level; the drawn bodies are the agent's footprint along the planned path.

IMAGE_SINGLE_AGENT

VIDEO_SINGLE_AGENT

Narrow passages: I-beam profile

An I-beam is wider than the gaps in the walls, so it has to turn to get through. Its footprint is a composite of the web and both flanges, so the planner checks the real profile at every heading and finds the manoeuvre a circle-based planner would miss.

VIDEO_IBEAM

Many agents

Several planners in one scene share a single map and search graph, each replanning on its own.

VIDEO_MULTIPLE_AGENTS

Documentation

- [Getting Started](Assets/Docs/GettingStarted.md) — from an empty scene to an agent driving toward a target.
- [Architecture](Assets/Docs/Architecture.md) — the assembly layout, layer rules, and where each piece of the pipeline lives.
- [Documentation home](Assets/Docs/README.md) — every page, grouped by architecture.
- [FAQ](Assets/Docs/FAQ.md) — common problems and how to fix them.

Architecture

See [Architecture](Assets/Docs/Architecture.md) for the assembly layout, layer rules, and where each piece of the pipeline lives.
I also use https://github.com/gregoryneal/ClothoidX to solve the G1 problem and describe the curvate function.

Status

(work in progress — add build/version/license badges here once decided)

Troubleshooting

Getting a DivideByZeroException or another problem? See the [FAQ](Assets/Docs/FAQ.md).
