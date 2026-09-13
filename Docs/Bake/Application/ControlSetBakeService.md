# ControlSetBakeService

The control set bake as a plain service: it assembles the chain of builders and runs it over a grid. [ControlSetBaker](../Presentation/ControlSetBaker.md) is the component that calls it.

`NCPF.Bake.Application` · `ControlSetBakeService.cs`

## Description

The service has no Unity dependencies and no logging. It takes the grid geometry and the bake settings, and returns a [ControlSet](../../Domain/ControlSet/ControlSet.md). Storing the result is the caller's job.

Each call builds this chain:

```
ControlSetBuilder                 bakes one quarter of the headings
 └─ BackwardLayerBuilder          adds reversing curves to every heading
     └─ LayerControlSetBuilderBase  bakes the forward curves of one heading
```

The forward curves come from the injected [IPathBuilder](IPathBuilder.md). The reversing curves use the same builder, wrapped so it solves them tail-first. A [PathLimiterBase](../../Domain/Agent/PathLimiterBase.md) built from the settings decides which curves are kept.

## Constructor

```csharp
ControlSetBakeService(IPathBuilder pathBuilder)
```

| Parameter | Purpose |
|---|---|
| `pathBuilder` | Produces the curve between two poses. Decides the family of curves in the library — clothoids by default. |

## Key API

```csharp
ControlSet<IPath> Bake(IGridGeometry3D space, float maxRadius, float lengthMultiplier,
                       float positionTube, float angleTube, float minTurnRadius)
```

Bakes a quarter of the headings and returns them as a control set. The remaining headings are rebuilt from these when the set is read.

| Parameter | Purpose |
|---|---|
| `space` | The grid the curves are placed on: cell size and heading step. |
| `maxRadius` | How far from the origin to generate curves, in metres. |
| `lengthMultiplier` | Curves longer than this many times the straight distance between their ends are rejected. |
| `positionTube` | How close, in metres, a curve must pass a shorter curve's end to be dropped as redundant. |
| `angleTube` | The same tolerance for heading, in degrees. |
| `minTurnRadius` | Curves turning tighter than this radius are rejected. `0` disables it. |

```csharp
IPath[] BakeLayer(IGridGeometry3D space, int angleLayer, float maxRadius, float lengthMultiplier,
                  float positionTube, float angleTube, float minTurnRadius)
```

Bakes the curves of a single heading, with the same settings. Faster than a full bake, for tuning.

## Related

- [ControlSetBaker](../Presentation/ControlSetBaker.md) — the component that runs it.
- [ControlSetBuilder](ControlSetBuilder.md) — the top of the chain.
- [IPathBuilder](IPathBuilder.md) — the injected curve family.
- [PathLimiterBase](../../Domain/Agent/PathLimiterBase.md) — the rules for keeping a curve.
