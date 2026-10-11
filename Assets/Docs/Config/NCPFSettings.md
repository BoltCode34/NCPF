# NCPFSettings

Package-wide runtime settings.

`NCPF.Config` · `NCPFSettings.cs`

## Description

Create one with **Create > NCPF > Settings**. For NCPF to find it, the asset must be named `NCPFSettings` and placed in a `Resources` folder of your project. The package does not ship one; the example project keeps one at `Assets/Resources/NCPFSettings.asset`.

The settings are optional. Without the asset, every setting keeps its default, so **Combine Dependencies** stays on.

It implements the [NcpfSettingsAsset](../Shared/NcpfSettingsAsset.md) port.

## Inspector

| Field | Default | Purpose |
|---|---|---|
| **Combine Dependencies** | On | In Play mode, planners that use the same map, control set, dynamics and preferences share one copy of the map, control set and search graph, including its cache. Turn it off to give every planner its own copies. See [NcpfPlannerBootstrap](../Pipeline/Presentation/NcpfPlannerBootstrap.md). |

## Related

- [NcpfSettingsAsset](../Shared/NcpfSettingsAsset.md) — the port it implements.
- [NcpfPlannerBootstrap](../Pipeline/Presentation/NcpfPlannerBootstrap.md) — reads the settings.
