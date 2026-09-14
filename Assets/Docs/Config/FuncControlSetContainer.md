# FuncControlSetContainer

The standard asset for storing a baked control set.

`NCPF.Config` · `FuncControlSetContainer.cs`

## Description

Create one with **Create > NCPF > Containers > FuncControlSetContainer**, then bake into it with [ControlSetBaker](../Bake/Presentation/ControlSetBaker.md). It implements the [ControlSetAsset](../Shared/ControlSetAsset.md) port.

Instead of storing sampled points, it stores each curve as a formula. Every curve's curvature is a cubic polynomial of the distance travelled, so a curve is five numbers — the four coefficients and its length — plus the node it ends on. A clothoid is the special case where the two highest coefficients are zero, so both kinds go through the same store.

Only a quarter of the headings is stored. When the set is read, the other three quarters are rebuilt by rotating the stored curves a quarter turn at a time. See [QuarterSymmetry](../Domain/ControlSet/QuarterSymmetry.md).

Reversing curves are stored alongside the forward ones in the same heading, marked as reversing.

The set lives only in the asset itself. An asset that has not been baked is empty, and reading it returns an empty control set.

## Key API

```csharp
ControlSet<IPath> Read(IGridGeometry3D space)
```

Rebuilds the full control set for the given grid. Clothoids are rebuilt exactly as the bake solved them; general cubic curves are integrated numerically.

```csharp
IPath[] ReadLayer(int angle, IGridGeometry3D space)
```

Rebuilds the curves of one heading.

```csharp
void Write(ControlSet<IPath> controlSet, IGridGeometry3D space)
```

Stores a control set, replacing the previous one. Only curves whose curvature is a cubic polynomial — [IPolynomialPath](../Domain/Core/IPath.md) — can be stored.

> **Caution:** the old contents are cleared before writing. If a curve of another kind is found, `Write` logs an error and stops, leaving the asset with only the headings written before that curve. Bake again after fixing the cause.

```csharp
int GetTotalPathCount()
bool HasLayer(int angle)
```

How many curves are stored in total, and whether a heading is stored.

## Related

- [ControlSetAsset](../Shared/ControlSetAsset.md) — the port it implements.
- [ControlSetBaker](../Bake/Presentation/ControlSetBaker.md) — bakes into it.
- [ControlSet](../Domain/ControlSet/ControlSet.md) — what it stores.
- [QuarterSymmetry](../Domain/ControlSet/QuarterSymmetry.md) — how the missing headings are rebuilt.
