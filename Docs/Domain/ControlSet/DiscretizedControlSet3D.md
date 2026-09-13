# DiscretizedControlSet3D

A control set in which every curve also carries the list of lattice cells it passes through. This is the form the search uses.

`NCPF.Domain` · `DiscretizedControlSet3D.cs`, `DiscretizedPath3D.cs`

## Description

```csharp
public class DiscretizedControlSet3D : ControlSet<DiscretizedPath3D> { }
```

Checking whether a curve fits at a node means checking the cells it covers. DiscretizedControlSet3D stores those cells alongside each curve, so they are computed once.

It is created from a plain control set by [DiscretizedControlSet3DConverter](../../Pipeline/Application/Search/DiscretizedControlSet3DConverter.md). Ids and layers work as in [ControlSet](ControlSet.md).

## DiscretizedPath3D

```csharp
public class DiscretizedPath3D : IPath, IPathWrapper
{
    readonly IPath Path;
    readonly Config[] IdSequence;
}
```

A curve plus its cells. It behaves as the curve itself for every [IPath](../Core/IPath.md) member.

| Field | Meaning |
|---|---|
| `Path` | The curve, starting at the origin. |
| `IdSequence` | Every distinct cell and heading the curve passes through, in order, relative to the start node. The last entry is where the curve ends. |

## Related

- [ControlSet](ControlSet.md) — layers and ids.
- [PassabilityBaker](../../Pipeline/Application/Space/PassabilityBaker.md) — checks the cells against the map.
