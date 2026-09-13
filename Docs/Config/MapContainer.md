# MapContainer

The standard asset for storing a baked map.

`NCPF.Config` · `MapContainer.cs`

## Description

Create one with **Create > NCPF > Containers > MapContainer**, then bake into it with [NCPFMapBuilder](../Bake/Presentation/NCPFMapBuilder.md). It implements the [MapAsset](../Shared/MapAsset.md) port.

The map is stored one heading at a time, each as a flat list of free or blocked cells, together with the grid size, the cell size and a description of the body it was baked for.

Its fields are hidden. Instead, the Inspector shows a preview: one heading at a time as an image, white where the body fits, with a slider to pick the heading. See [Bake editors](../Bake/Editor/BakeEditors.md).

## Key API

```csharp
Map3D Read()
```

Rebuilds the stored map as a [Map3D](../Domain/Collision/Map3D.md). If nothing has been baked yet, it returns an empty map with no headings.

```csharp
void Write(Map3D map)
```

Stores a map, replacing the previous one.

## Notes

- **An empty container breaks planning.** A map that was never baked has zero headings and zero cells, and turning a position into a lattice node divides by those counts. If you see a `DivideByZeroException` when entering Play mode, bake the map. See the [FAQ](../FAQ.md).
- **Nothing checks that the map matches the body.** The container stores a description of the body it was baked for, but nothing compares it with the shape you assign elsewhere. A map baked for a different shape or anchor is used without any warning, so re-bake whenever the shape changes.

## Related

- [MapAsset](../Shared/MapAsset.md) — the port it implements.
- [NCPFMapBuilder](../Bake/Presentation/NCPFMapBuilder.md) — bakes into it.
- [Map3D](../Domain/Collision/Map3D.md) — what it stores.
