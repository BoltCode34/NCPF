# PathAgentPipeline

The planning BRAIN: it owns the geometric search ([[PathFindingAgent]]) and the path follower ([[PathFollowAgent]]) and runs the uninterrupted search loop — target aging, replan throttling (wall-clock outside play mode), waiting out an in-process search, consuming a buffered result with a stitched hot/cold plan swap, and feeding the follower. The shell hands in the body's live state each tick; the brain never touches a transform. The path cursor lives here.

`NCPF.Pipeline.Application · Agent/PathAgentPipeline.cs`

## What it is

NEW in the decomposition — the old `PathAgentPipeline` was split into this brain and the follower [[PathFollowAgent]]. This class orchestrates; the follower executes.

## Constructor

```csharp
PathAgentPipeline(PathFindingAgent search, PathFollowAgent follower, IGridMap3D map,
                  ITarget target, float replanInterval, DynamicState? rebuildSeed)
```

`search` — the [[PathFindingAgent]]: builds the geometric path (async, ε-ladder).

`follower` — the [[PathFollowAgent]]: drives the model from the geometry.

`map` — the [[IGridMap3D]]: node space for target staleness check.

`target` — the [[ITarget]]: what we're going after.

`replanInterval` — wall-clock seconds between replans (uses `Time.time` in play, `Time.realtimeSinceStartup` in editor).

`rebuildSeed` — a state that survived a map rebuild; used for cold start after rebuild.

## Key API

```csharp
void Update(DynamicState bodyState)
```
One tick of the search loop:
1. **Staleness check** — plan is stale if empty or its end is > 2 cells from target.
2. **InProcess guard** — if search is busy (goal building or path finding), wait.
3. **Buffered path?** — if the search has a ready result, stitch and hand to follower.
4. **Replan throttle** — if enough wall time passed since last plan, start a new search from current body state to target.

```csharp
PathFollowAgent Follower / CurvateTransition[] Path / DynamicState? RebuildSeed
```
Read-only: the follower, the current geometric plan (for drawing), and the rebuild seed.

```csharp
void Break()
```
Releases the in-flight search and drops the plan — the next Update rebuilds it.

## Stitching (SetUpPath)

`StitchPath` takes the buffered result and stitches it to the old plan's current primitive (the body is on it; its end is the anchor). If the follower already has a path, it hot-swaps with state saved; otherwise cold-starts from the body state (or rebuild seed). Failed stitch discards the result — raw application would teleport the body back.

## Related

- [[PathFollowAgent]] — the follower (app)
- [[PathFindingAgent]] — the search (app)
- [[BehaviorBus]] — behaviour slots (app)
- [[PathAgentContext]] — the live dependencies
- [[IGridMap3D]] — node space
- [[ITarget]] — target contract