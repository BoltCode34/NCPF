# IPathBuilder

Produces the curve between two poses. The path builder decides which family of curves the control set is made of.

`NCPF.Bake.Application` · `IPathBuilder.cs`, `ClothoidPathBuilder.cs`, `RearFirstPathBuilder.cs`

## Description

The bake asks a path builder for a curve from the origin pose to each candidate target pose. The curve must start and end exactly at the given positions and headings.

`IPathBuilder` has a single method:

```csharp
IPath Create(WorldConfig start, WorldConfig end)
```

The package includes two builders.

### ClothoidPathBuilder

The default. Produces a [ClothoidPath](../../Domain/Paths/ClothoidPath.md) — an Euler spiral, a curve whose curvature changes linearly along its length — by solving the G1 Hermite problem with the Bertolazzi–Frego method: match both positions and both headings exactly. Clothoids are what roads are built from, because steering can follow them smoothly.

### RearFirstPathBuilder

Produces reversing curves. It turns both headings by 180°, asks another builder for the curve, and wraps the result in a [RearFirstPath](../../Domain/Paths/RearFirstPath.md) that turns the heading back. A 180° turn keeps every lattice node on a lattice node, so the reversing curves still end exactly on nodes at every heading.

```csharp
RearFirstPathBuilder(IPathBuilder forward)
```

| Parameter | Purpose |
|---|---|
| `forward` | Solves the curve with the turned headings. It must produce an [IPolynomialPath](../../Domain/Core/IPath.md). |

## Writing your own

Implement `Create` and pass your builder to [ControlSetBakeService](ControlSetBakeService.md), for example through `ControlSetBaker.Construct`.

To be storable by [FuncControlSetContainer](../../Config/FuncControlSetContainer.md), your curves must be [IPolynomialPath](../../Domain/Core/IPath.md): their curvature must be a cubic polynomial of the distance travelled.

## Related

- [ControlSetBakeService](ControlSetBakeService.md) — where the builder is injected.
- [ClothoidPath](../../Domain/Paths/ClothoidPath.md) — the default curve.
- [RearFirstPath](../../Domain/Paths/RearFirstPath.md) — the reversing wrapper.
