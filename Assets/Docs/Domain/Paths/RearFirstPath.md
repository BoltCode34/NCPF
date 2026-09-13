# RearFirstPath

A curve driven in reverse: the body moves along it with its back first.

`NCPF.Domain` · `RearFirstPath.cs`

## Description

When a body reverses, it faces one way and moves the opposite way. RearFirstPath wraps a curve that describes the **motion** and reports headings turned by 180°, so the headings it returns are where the body **faces**, as every other path does. Positions, length and curvature are unchanged.

The bake builds a reversing curve by solving an ordinary curve between the two poses with both headings flipped, then wrapping it. Because a 180° turn maps lattice headings onto lattice headings, the curve still ends exactly on a lattice node.

The type also carries the `IRearFirstPath` marker, which is how [PathGearUtility](../Agent/PathGearUtility.md) recognises reversing curves, even when they are wrapped by other paths.

## Constructor

```csharp
RearFirstPath(IPolynomialPath inner)
```

| Parameter | Purpose |
|---|---|
| `inner` | The curve of motion, with headings pointing the way the body travels. |

## Related

- [PathGearUtility](../Agent/PathGearUtility.md) — detects reversing curves.
- [BackwardLayerBuilder](../../Bake/Application/BackwardLayerBuilder.md) — bakes reversing curves.
- [RearGearGraph](../../Pipeline/Application/Space/RearGearGraph.md) — makes them more expensive in the search.
