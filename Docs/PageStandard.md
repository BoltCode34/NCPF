# Page Standard

How a page in these docs is written. The reference for tone is the Unity documentation: say what a thing is first, then the details, and nothing more than the reader needs.

This page is for people contributing to the documentation. If you are here to use the package, start with [Getting Started](GettingStarted.md).

## Where pages live

All documentation lives in `Assets/Docs`. Object pages are grouped the same way the code is grouped — by assembly, then by the folder inside it:

```
Assets/Docs/Domain/Core/IPath.md            ↔  Assets/Logic/NCPF/Domain/Core/IPath.cs
Assets/Docs/Pipeline/Presentation/PathPlanner.md  ↔  Assets/Logic/NCPF/Pipeline/Presentation/PathPlanner.cs
```

A type gets its own page when a user works with it or extends it. Very small types that only exist to serve one owner — a nested DTO, a marker interface, a two-field struct — are described on their owner's page instead.

## Language and links

- English only.
- Links are regular relative Markdown links, so they work on GitHub and in any Markdown viewer: `[PathPlanner](../../Pipeline/Presentation/PathPlanner.md)`. Do not use `[[wiki links]]`.
- Refer to code identifiers in backticks: `PathPlanner.Construct`.

## Required sections

### Title and summary

The type name as the title, then one or two sentences: what the object is and why it exists. Below that, the namespace and file:

```
# PathPlanner

The component you put on an agent to make it plan and drive toward a target.

`NCPF.Pipeline.Presentation` · `PathPlanner.cs`
```

### Constructor

Required when the constructor takes dependencies (other objects, interfaces, services), or when it takes more than four parameters of any kind.

For each parameter, say **what it is for here**, not what its type is.

```
PathFindingAgent(IGridMap3D map3D, ICurvateGraph graph, ...)

map3D   — converts the body's world pose into a lattice node to search from
graph   — the graph the search walks
```

Components configured in the Inspector have no constructor to document. Describe their Inspector fields in the same way instead.

### Key API

The public members that matter. A member matters when:

- it is how you mainly talk to the object (for example, `FindPath` on a finder), or
- three or more other types use it.

One block per member:

```
FindPath(IGraph<T> graph, int from, IGoal goal, int maxIterations)

Runs A* from a node until the goal is achieved or the iteration budget runs out.

Parameters
graph          — the graph to search
from           — the start node id
goal           — decides when to stop and estimates the remaining cost
maxIterations  — how many nodes may be expanded before giving up

Returns
The edges of the path, or an empty array if none was found.
```

When there are many similar members, a table is fine, but the key ones still get a full block.

### Related

A list of links to neighbouring types at the end of the page, each with a few words on how they relate.

## Optional sections

- **Notes** — traps, non-obvious behaviour, things that are easy to get wrong.
- **How it works** — a short mechanical explanation, when the behaviour is not obvious from the API.
