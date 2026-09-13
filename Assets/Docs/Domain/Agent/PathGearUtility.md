# PathGearUtility

Tells whether a curve is driven forward or in reverse, and in which direction the body moves on it.

`NCPF.Domain` · `PathGearUtility.cs`

## Description

A path's heading is where the body faces. On a reversing curve, the body moves the opposite way. PathGearUtility answers both questions for any curve, looking through wrappers such as [PathOffset](../../Pipeline/Application/Space/PathOffset.md) to find a [RearFirstPath](../Paths/RearFirstPath.md) inside.

## Key API

```csharp
static bool IsRearFirst(IPath path)
static int GearOf(IPath path)
```

Whether the curve is driven in reverse, and the same as a sign: `Forward` (`+1`) or `Rear` (`−1`). `null` counts as forward.

```csharp
static float TravelAngle(IPath path, float arc)
```

The direction the body moves at `arc` metres, in degrees: the curve's heading, turned by 180° on a reversing curve.

## Notes

- Reversing is recognised only through the `IRearFirstPath` marker and `IPathWrapper`. A wrapper of your own that does not implement `IPathWrapper` hides a reversing curve, which is then driven forward.

## Related

- [RearFirstPath](../Paths/RearFirstPath.md) — the reversing curve.
- [RearGearGraph](../../Pipeline/Application/Space/RearGearGraph.md) — uses it in the search.
