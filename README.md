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
Screenshots

<img width="476" height="377" alt="image" src="https://github.com/user-attachments/assets/e4d47458-14e3-436b-8550-abfe5f9d3a52" />

<img width="523" height="598" alt="image" src="https://github.com/user-attachments/assets/c1751b1e-cecf-4d02-9182-f6732bf2d8ef" />


<img width="434" height="386" alt="image" src="https://github.com/user-attachments/assets/a766f39a-4ef0-4751-9b30-49487334ed2f" />

<img width="750" height="352" alt="image" src="https://github.com/user-attachments/assets/35691fb9-0272-4354-a823-47776170857b" />

Architecture

See ARCHITECTURE.md for the assembly layout, layer rules, and where each piece of the pipeline lives.
I also use https://github.com/gregoryneal/ClothoidX to solve the G1 problem and describe the curvate function.
Status

(work in progress — add build/version/license badges here once decided)
Warning
if you get divide by xero exeption
Pls execute on scene NCPFMap -> NCPFMapBuilder->Build(in context menu)
