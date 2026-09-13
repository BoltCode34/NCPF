# LayerControlSetBuilderBase

Bakes the curves that start at one heading: grows rings of target cells outward from the origin and keeps only the curves that cannot be assembled from shorter ones.

`NCPF.Bake.Application` · `LayerControlSetBuilderBase.cs`

## Description

This is where the control set is actually decided. It implements `ILayerControlSetBuilder`:

```csharp
IPath[] BuildLayer(IGridGeometry3D gridSpace, int maxCell, WorldConfig origin)
```

### How it works

1. **Grow rings.** Starting one cell away, it takes every cell within the current radius that has not been visited yet — a ring. It repeats for every radius from 1 cell up to `maxCell`.
2. **Generate candidates.** For each cell in the ring it asks the [IPathBuilder](IPathBuilder.md) for curves from the origin to that cell, ending at a few headings around the direction of the straight line to the cell. A curve that arrives moving sideways is never a useful short move, so only headings close to that direction are tried.
3. **Reject broken curves.** A candidate whose length is not a finite positive number, or whose end does not land exactly on the intended node, is discarded.
4. **Apply the limits.** The [IPathLimiter](../../Domain/Agent/PathLimiterBase.md) rejects curves that are too long, too tight, or pass close enough to an already kept node to be made of shorter curves.
5. **Check again against the whole ring.** All candidates in a ring are registered first, and only then checked for being made of shorter curves, so candidates of the same ring can rule each other out.

The surviving curves of every ring make up the heading's layer.

### Differences from the published algorithm

In Pivtoraiko & Kelly's method, the rings stop growing as soon as a whole ring contributes no new curve. This implementation does not stop early: **it always grows rings out to `maxCell`**. The maximum radius is therefore the real end of the bake, not just a safety limit, and it directly sets the bake time.

## Constructor

```csharp
LayerControlSetBuilderBase(IPathLimiter limiter, IPathBuilder pathBuilder, int endHeadingSpread = 2)
```

| Parameter | Purpose |
|---|---|
| `limiter` | Decides which candidate curves are kept. |
| `pathBuilder` | Produces the curve from the origin to a target pose. |
| `endHeadingSpread` | How many headings on each side of the straight-line direction are tried at each target cell. |

## Key API

```csharp
IPath[] BuildLayer(IGridGeometry3D gridSpace, int maxCell, WorldConfig origin)
```

Bakes the curves starting at `origin`.

| Parameter | Purpose |
|---|---|
| `gridSpace` | The grid the curves are placed on. |
| `maxCell` | How many rings to grow, in cells. |
| `origin` | The start pose: the origin cell and the heading being baked. |

## Extending

The steps above are virtual, so a derived builder can change one without rewriting the rest: `GenerateRadius`, `GenerateCandidates`, `CreatePath`, `GeneratePosition`, `HeadingCenter` and `IsSanePrimitive`.

`RearFirstLayerBuilder` is an example. It overrides only `HeadingCenter` to aim the tried headings 180° away, because a body reversing into a cell arrives facing away from it.

## Related

- [BackwardLayerBuilder](BackwardLayerBuilder.md) — runs this builder forward and in reverse.
- [PathLimiterBase](../../Domain/Agent/PathLimiterBase.md) — the rules for keeping a curve.
- [IPathBuilder](IPathBuilder.md) — produces the candidate curves.
- [ControlSetBuilder](ControlSetBuilder.md) — calls it once per heading.
