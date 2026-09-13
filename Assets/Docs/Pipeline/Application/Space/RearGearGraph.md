# RearGearGraph

A graph decorator that makes curves driven in reverse more expensive, so the agent reverses only when it is clearly worth it.

`NCPF.Pipeline.Application` · `RearGearGraph.cs`

## Description

To the motion model, reversing is as legal and as fast as driving forward. To most characters it is not natural: they look where they go. RearGearGraph multiplies the weight of every reversing curve by a penalty, so casual reversing loses while a reverse that saves a long detour still wins.

Which curves reverse is determined once per curve in the constructor.

## Constructor

```csharp
RearGearGraph(ICurvateGraph decoratee, DiscretizedControlSet3D controlSet, float gearPenalty)
```

| Parameter | Purpose |
|---|---|
| `decoratee` | The graph whose weights to adjust. |
| `controlSet` | The curves, to find which ones reverse. |
| `gearPenalty` | The multiplier for reversing curves. `1` treats reversing like driving forward; values below `1` are raised to `1`. |

## Notes

- It modifies the edge array returned by the decoratee **in place**, so it must sit below [CachedCurvateGraph](CachedCurvateGraph.md).

## Related

- [PathGearUtility](../../../Domain/Agent/PathGearUtility.md) — decides which curves reverse.
- [ICurvateGraph](ICurvateGraph.md) — the standard stack.
