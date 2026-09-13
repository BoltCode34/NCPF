# BehaviorBus

Holds the agent's live behaviour in swappable slots, so a behaviour can be replaced while the search and the follower keep running.

`NCPF.Pipeline.Application` · `BehaviorBus.cs`

## Description

The search and the follower are built once and hold on to their main pass and goal builders. To change behaviour without rebuilding them, the bus gives them stand-ins instead of the real objects:

| Slot | Stand-in | Given to |
|---|---|---|
| `Main` | `RefMainPlanner` | the follower, as its main pass |
| `Achivement` | `RefAchivementBuilder` | the goal builder of the search |
| `Heuristic` | `RefHeuristicBuilder` | the goal builder of the search |

Each stand-in forwards to a real object that can be replaced at any time. Replacing it takes effect on the next call, while everything keeps running.

Override passes are added to and removed from the follower's pass list directly.

The bus speaks only in terms of passes and builders. It does not know about the Inspector features that create them; [PathFeatureContainer](../../../Pipeline/Presentation/PathFeatureContainer.md) does that translation.

## Key API

```csharp
void SetMain(MainSAPass pass)
void SetAchivement(IAchivementBuilder builder)
void SetHeuristic(IHeuristicBuilder builder)
```

Replace the logic behind a slot.

```csharp
void Attach(PathFollowAgent follower)
```

Connects the follower whose pass list the override methods change. Call it once, before the first `AddOverride`.

```csharp
void AddOverride(SAPass pass)
void RemoveOverride(SAPass pass)
IReadOnlyList<SAPass> Overrides { get; }
```

Add or remove an optional pass, and list the active ones.

## Notes

- The heuristic stand-in forwards the asynchronous build path directly, so an expensive heuristic that builds on a background thread keeps doing so behind the bus.

## Related

- [PathFeatureContainer](../../../Pipeline/Presentation/PathFeatureContainer.md) — fills the slots from features.
- [PathFollowAgent](PathFollowAgent.md) — uses the main slot and the overrides.
- [IGoalBuilder](../Search/IGoalBuilder.md) — uses the achievement and heuristic slots.
