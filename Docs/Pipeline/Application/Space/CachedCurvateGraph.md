# CachedCurvateGraph

A graph decorator that remembers each node's edges and free flag, so repeated searches over the same area do not recompute them.

`NCPF.Pipeline.Application` · `CachedCurvateGraph.cs`

## Description

A node's edges depend only on the node and the static map. Yet every rung of the [epsilon ladder](../Search/EpsilonLadderFinder.md) and every replan asks for the same nodes again. CachedCurvateGraph stores the answer the first time and returns it afterwards.

The cache is safe to use from several searches at once. It lives as long as the graph object: when the map is rebuilt, the planner composes a new graph and the old cache goes with the old graph.

The cache is limited by the total number of stored **edges**. When the limit is exceeded, the whole cache is cleared and fills up again.

## Constructor

```csharp
CachedCurvateGraph(ICurvateGraph decoratee, int edgeBudget = 2000000)
```

| Parameter | Purpose |
|---|---|
| `decoratee` | The graph to cache. |
| `edgeBudget` | The maximum number of cached edges. An edge takes 16 bytes, so the default is about 32 MB. |

## Notes

- **Keep it outermost.** Decorators such as [RearGearGraph](RearGearGraph.md) change weights in the array they return. Above the cache, that change would be applied to the cached array again on every visit.
- **Treat returned arrays as read-only.** They are shared between searches.

## Related

- [ICurvateGraph](ICurvateGraph.md) — the standard stack.
- [NcpfGraphComposer](../../../Pipeline/Presentation/NcpfGraphComposer.md) — composes it.
