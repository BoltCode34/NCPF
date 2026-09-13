# Bake logging

A set of decorators that record every decision of a control set bake — which curves were tried, which checks each one passed, and which were kept — for diagnosing a library that looks wrong.

`NCPF.Bake.Application` · `IControlSetBuildLogger.cs`, `ControlSetBuilderLogDecorator.cs`, `LayerBuilderLog.cs`, `LimiterLog.cs`, `BakeLog.cs`

## Description

The default bake does not log. To log, compose the bake chain yourself with the logging versions of its parts, all reporting to one `IControlSetBuildLogger`:

| Type | Replaces | Records |
|---|---|---|
| `ControlSetBuilderLogDecorator<T>` | wraps a [ControlSetBuilder](ControlSetBuilder.md) | the start of each bake |
| `LayerBuilderLog` | [LayerControlSetBuilderBase](LayerControlSetBuilderBase.md) | each heading, each ring and each candidate curve |
| `LimiterLog` | [PathLimiterBase](../../Domain/Agent/PathLimiterBase.md) | the outcome of every check on a curve, and the nearest nodes it passed |

The records are plain structs defined in `BakeLog.cs`: `BuildLog`, `LayerLog`, `PathLog` and `LimiterPathLog`. They are geometric only — poses and lattice cells.

[ControlSetBuildLogger](../Presentation/ControlSetBuildLogger.md) is the standard logger: an asset that keeps the last few bakes and can write them to a file.

## IControlSetBuildLogger

```csharp
void StartLogGeneration()
void StartLogLayer(WorldConfig origin, Config originCell)
void LogPath(Config endCell, WorldConfig end, float radius = -1, float length = -1, bool includedInResult = false)
void LogCheckingPath(LimiterPathLog log, Config pos)
void MarkLayerAsIncluded(Config end)
void SaveLog()
```

Called in that order during a bake: once per bake, once per heading, once per candidate curve, once per limit check. `SaveLog` writes what was recorded.

## Constructors

```csharp
ControlSetBuilderLogDecorator(IControlSetBuilder<T> decoratee, IControlSetBuildLogger logger)
LayerBuilderLog(IControlSetBuildLogger logger, IPathLimiter limiter, IPathBuilder factory, int endHeadingSpread = 2)
LimiterLog(IControlSetBuildLogger logger, float breakDistance = 1.4f, float positionTube = 0.25f, float angleTube = 10f)
```

Each takes the logger first, then the same parameters as the part it replaces.

## Related

- [ControlSetBuildLogger](../Presentation/ControlSetBuildLogger.md) — the standard logger asset.
- [ControlSetBakeService](ControlSetBakeService.md) — the default, non-logging bake chain.
