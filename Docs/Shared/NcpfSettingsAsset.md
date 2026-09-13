# NcpfSettingsAsset

The port for package-wide runtime settings.

`NCPF.Shared.Presentation` · `NcpfSettingsAsset.cs`

## Description

NcpfSettingsAsset is an abstract ScriptableObject. [NcpfPlannerBootstrap](../Pipeline/Presentation/NcpfPlannerBootstrap.md) loads it from a `Resources` folder under the name `NCPFSettings`. The standard implementation is [NCPFSettings](../Config/NCPFSettings.md).

The bootstrap reads the settings through this port so that it does not depend on the assembly with the concrete asset.

## Key API

```csharp
bool CombineDependencies { get; }
```

Whether planners with the same assets, dynamics and preferences share one copy of their map, control set and search graph.

## Related

- [NCPFSettings](../Config/NCPFSettings.md) — the standard implementation.
- [NcpfPlannerBootstrap](../Pipeline/Presentation/NcpfPlannerBootstrap.md) — reads the settings.
