# Bake editors

Inspector tools for the bake assets. They live in an Editor-only assembly and are not included in builds.

`NCPF.Bake.Editor`

## NCPFMapEditor

`NCPFMapEditor.cs`

Replaces the Inspector of any [MapAsset](../../Shared/MapAsset.md), including [MapContainer](../../Config/MapContainer.md). It shows the number of headings, the angle between them and the grid size, and draws one heading of the baked map as an image: white where the body fits, black where it does not. The **View Layer** slider picks the heading.

It reads the concrete asset's fields by reflection, so the Editor assembly does not need to reference `NCPF.Config`.

## AgentConfigEditor

`AgentConfigEditor.cs`

Adds a preview to every [AgentConfig](../../Shared/AgentConfig.md), including your own: the body as the grid of cells it will be baked as, with the anchor cell highlighted, at a chosen cell size and heading. The anchor cell is the one placed on the agent's position, so it must match the point your agent's GameObject is moved by. See [AgentConfig](../../Shared/AgentConfig.md#inspector-preview).

## InlineFieldDrawer

`InlineFieldDrawer.cs`

Draws fields marked with `[InlineField]` as a foldout whose label shows values from inside the folded object, for example `X: {EndCell.X} Y: {EndCell.Y}`. Used by the bake log to make long lists of records readable without opening each one. Types can implement `IInlineInspectable` to opt in without a template.

## Related

- [Shared editors](../../Shared/Editor/SharedEditors.md) — the type picker used by both halves.
