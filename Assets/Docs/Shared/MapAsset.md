# MapAsset

The storage port for a baked map. Any field that needs a baked map is typed as MapAsset, so the storage format can change without the code that uses it noticing.

`NCPF.Shared.Presentation` · `MapAsset.cs`

## Description

MapAsset is an abstract ScriptableObject. The standard implementation is [MapContainer](../Config/MapContainer.md). When you drag a MapContainer into a field of type MapAsset, Unity binds the concrete asset, and every `Read` and `Write` reaches it.

To store maps differently — in a file, a compressed format, a database — derive from MapAsset in your own assembly. Nothing else needs to change. See [Architecture](../Architecture.md#ports-why-nothing-references-config).

## Key API

```csharp
Map3D Read()
```

Returns the stored map as a [Map3D](../Domain/Collision/Map3D.md).

```csharp
void Write(Map3D map)
```

Stores a map. Called by [NCPFMapBuilder](../Bake/Presentation/NCPFMapBuilder.md) after a bake.

## Related

- [MapContainer](../Config/MapContainer.md) — the standard implementation.
- [Map3D](../Domain/Collision/Map3D.md) — what is stored.
- [ControlSetAsset](ControlSetAsset.md) — the matching port for the control set.
