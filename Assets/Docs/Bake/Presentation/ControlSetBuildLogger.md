# ControlSetBuildLogger

An asset that records what the control set bake decided about each candidate curve, for diagnosing why a curve was kept or dropped.

`NCPF.Bake.Presentation` · `ControlSetBuildLogger.cs`

## Description

Create one with **Create > Logs > Control set**. It stores the last few bakes as a list of layers, curves and the checks each curve went through, and can write them to a text file.

It implements [IControlSetBuildLogger](../Application/BakeLogging.md). It is **not used by the default bake**: [ControlSetBaker](ControlSetBaker.md) bakes without logging. To use it, compose a bake chain with the logging decorators described in [Bake logging](../Application/BakeLogging.md) and pass this asset as the logger.

## Inspector

| Field | Purpose |
|---|---|
| **Buffer** | How many bakes to keep. Older ones are dropped. |
| **Logs** | The recorded bakes. |

## Key API

```csharp
void SaveLog()
```

Writes the recorded bakes to a readable text file. Also available from the asset's context menu as **Save Log To File**.

## Related

- [Bake logging](../Application/BakeLogging.md) — the logging contract and decorators.
- [ControlSetBuilder](../Application/ControlSetBuilder.md) — the bake being logged.
