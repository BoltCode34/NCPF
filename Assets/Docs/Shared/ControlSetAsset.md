# ControlSetAsset

The storage port for a baked control set. Any field that needs the primitive library is typed as ControlSetAsset, so the storage format can change without the code that uses it noticing.

`NCPF.Shared.Presentation` · `ControlSetAsset.cs`

## Description

ControlSetAsset is an abstract ScriptableObject. The standard implementation is [FuncControlSetContainer](../Config/FuncControlSetContainer.md).

Reading and writing both take the grid geometry, because the stored curves are placed on lattice nodes: to rebuild them exactly, the store needs the grid's cell size and heading step.

## Key API

```csharp
ControlSet<IPath> Read(IGridGeometry3D space)
```

Returns the full control set, every heading, for the given grid.

```csharp
IPath[] ReadLayer(int angle, IGridGeometry3D space)
```

Returns the curves of one heading only. Cheaper than a full read when you only need one, for example to draw it.

```csharp
void Write(ControlSet<IPath> controlSet, IGridGeometry3D space)
```

Stores a control set. Called by [ControlSetBaker](../Bake/Presentation/ControlSetBaker.md) after a bake.

## Related

- [FuncControlSetContainer](../Config/FuncControlSetContainer.md) — the standard implementation.
- [ControlSet](../Domain/ControlSet/ControlSet.md) — what is stored.
- [MapAsset](MapAsset.md) — the matching port for the map.
