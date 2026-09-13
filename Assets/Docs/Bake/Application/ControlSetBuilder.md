# ControlSetBuilder

Bakes a control set by baking each heading in one quarter of a turn.

`NCPF.Bake.Application` · `ControlSetBuilder.cs`

## Description

A curve is defined by how its curvature changes along it, and that does not depend on which way the curve points. So a set of curves starting at heading 0° only needs rotating by 90° to serve heading 90°. ControlSetBuilder uses this: it bakes only the headings in the first quarter of a turn, from the origin cell, and leaves the other three quarters to be rebuilt when the set is read. See [QuarterSymmetry](../../Domain/ControlSet/QuarterSymmetry.md).

The work of baking a single heading is delegated to an [ILayerControlSetBuilder](LayerControlSetBuilderBase.md).

It implements `IControlSetBuilder<IPath>`, the contract for anything that bakes a whole control set:

```csharp
ControlSet<T> Create(IGridGeometry3D gridSpace, float maxRadius)
```

## Constructor

```csharp
ControlSetBuilder(ILayerControlSetBuilder layerBuilder)
```

| Parameter | Purpose |
|---|---|
| `layerBuilder` | Bakes the curves of one heading. |

## Key API

```csharp
ControlSet<IPath> Create(IGridGeometry3D gridSpace, float maxRadius)
```

Bakes every heading in the first quarter of a turn.

| Parameter | Purpose |
|---|---|
| `gridSpace` | The grid: cell size and heading step. The number of headings must be a multiple of 4. |
| `maxRadius` | How far from the origin to generate curves, in metres. |

Returns a control set containing only the baked quarter.

## Related

- [ControlSetBakeService](ControlSetBakeService.md) — builds and runs it.
- [LayerControlSetBuilderBase](LayerControlSetBuilderBase.md) — bakes one heading.
- [QuarterSymmetry](../../Domain/ControlSet/QuarterSymmetry.md) — rebuilds the other headings.
- [Bake logging](BakeLogging.md) — a decorator that logs a bake.
