# PrimitivePassability

A bit mask recording, for every lattice pose, which curves of its heading layer fit there.

`NCPF.Domain` · `PrimitivePassability.cs`

## Description

A curve placed at a pose has the same shape wherever it is placed; the only thing that changes from pose to pose is whether it hits an obstacle. PrimitivePassability stores just that: one bit per pose and curve. A whole map costs megabytes, where storing the placed curves would cost gigabytes.

A curve is identified by its **position in its heading layer**, not by its id. So a mask only makes sense together with the control set it was baked from.

It is filled by [PassabilityBaker](../../Pipeline/Application/Space/PassabilityBaker.md) and read by [PassabilityTransitionGenerator](../../Pipeline/Application/Space/PassabilityTransitionGenerator.md).

## Constructor

```csharp
PrimitivePassability(int nodeCount, int primitiveCapacity)
```

| Parameter | Purpose |
|---|---|
| `nodeCount` | The number of node ids. |
| `primitiveCapacity` | The number of curves in the largest layer. |

## Key API

```csharp
bool Passable(int nodeId, int primitiveIndex)
void Set(int nodeId, int primitiveIndex, bool passable)
```

Reads and writes one bit. Out-of-range reads return `false` instead of throwing, so a mask that does not match its control set leads to a search that finds nothing, not to an exception on a background thread.

## Notes

- **Write once, then only read.** Reading from several searches at once is safe; writing while searches run is not.

## Related

- [PassabilityBaker](../../Pipeline/Application/Space/PassabilityBaker.md) — fills it.
- [ControlSet](../ControlSet/ControlSet.md) — the layers it refers to.
