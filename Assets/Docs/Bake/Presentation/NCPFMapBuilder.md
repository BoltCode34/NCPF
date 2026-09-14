# NCPFMapBuilder

Bakes the map: for every cell of the level and every heading, whether the agent's body fits there. The map is the first thing you bake on a new level.

`NCPF.Bake.Presentation` · `NCPFMapBuilder.cs`

## Description

NCPFMapBuilder reads the level's obstacles through a [Unity2DGrid](../../Shared/Unity2DGrid.md), takes the body shape from an [AgentConfig](../../Shared/AgentConfig.md), and asks [MapBuilder](../../Domain/Collision/MapBuilder.md) to compute where that body fits at each heading. The result, a [Map3D](../../Domain/Collision/Map3D.md), is written into a [MapAsset](../../Shared/MapAsset.md) and saved.

A cell counts as free at a heading only if the whole body, rotated to that heading and placed by its anchor on that cell, overlaps no obstacle. This is what lets the search treat a long body correctly: a gap can be free for the body turned one way and blocked for it turned another.

To bake, right-click the component header and select **Build**.

## Inspector

| Field | Purpose |
|---|---|
| **Map** | The [MapAsset](../../Shared/MapAsset.md) to bake into, usually a [MapContainer](../../Config/MapContainer.md). |
| **Agent** | The body shape, an [AgentConfig](../../Shared/AgentConfig.md) such as [RectangularConfig](../../Config/RectangularConfig.md) or [CompositeConfig](../../Config/CompositeConfig.md). |
| **Grid** | The [Unity2DGrid](../../Shared/Unity2DGrid.md) to read obstacles from. |
| **Angle Step Count** | How many headings the map distinguishes. **Must be a multiple of 4**, because the control set is baked for one quarter of a turn and rotated into the other three. 16 (steps of 22.5°) is a good default. |
| **Build On Awake** | Bakes again every time the scene starts. Leave it off unless your obstacles change at runtime: a bake takes noticeable time and rewrites the asset. |

## Key API

```csharp
void Build()
```

Re-reads the obstacles from the grid, bakes the map for the configured body and heading count, and writes it into the map asset. In the Editor, the asset is saved to disk. Also available from the component's context menu.

## Notes

- **Bake again when the level changes.** The map does not update on its own. Moving obstacles, changing the grid's size, cell size or obstacle layers, or changing the body shape or its anchor all require a new bake.
- **One map per body shape.** A map is only correct for the body it was baked with. Agents with different shapes need separate map assets.
- The [ControlSetBaker](ControlSetBaker.md) reads the map's grid, so bake the map before the control set.

## Related

- [ControlSetBaker](ControlSetBaker.md) — the second bake step.
- [MapBuilder](../../Domain/Collision/MapBuilder.md) — does the actual computation.
- [MapContainer](../../Config/MapContainer.md) — stores the result; its Inspector shows a preview.
- [AgentConfig](../../Shared/AgentConfig.md) — the available body shapes.
- [Getting Started](../../GettingStarted.md) — step 3 walks through a bake.
