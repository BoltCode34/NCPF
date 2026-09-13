# ControlSetBaker

Bakes the control set: the library of short curves the agent can drive. You bake it after the map.

`NCPF.Bake.Presentation` · `ControlSetBaker.cs`

## Description

ControlSetBaker reads the grid geometry from the baked map, runs [ControlSetBakeService](../Application/ControlSetBakeService.md), and writes the resulting [ControlSet](../../Domain/ControlSet/ControlSet.md) into a [ControlSetAsset](../../Shared/ControlSetAsset.md), which is then saved.

The baker generates candidate curves from the origin to cells further and further away, and keeps only the ones that cannot be assembled from shorter curves it already has. The result is a small library where every curve adds a move that is not otherwise possible. Only a quarter of the headings is baked; the other three quarters are rotations of it and are rebuilt when the set is read.

To bake, right-click the component header and select **Bake**.

## Inspector

### References

| Field | Purpose |
|---|---|
| **Map Container** | The baked [MapAsset](../../Shared/MapAsset.md). Only its heading count is used: the control set is pure geometry and ignores obstacles. |
| **Control Set Container** | The [ControlSetAsset](../../Shared/ControlSetAsset.md) to bake into, usually a [FuncControlSetContainer](../../Config/FuncControlSetContainer.md). |
| **Grid** | The [Unity2DGrid](../../Shared/Unity2DGrid.md) the map was baked on. Provides the cell size. |

### Bake settings

These decide which curves end up in the library. See [PathLimiterBase](../../Domain/Agent/PathLimiterBase.md) for the exact rules.

| Field | Purpose |
|---|---|
| **Max Radius** | How far from the origin the baker generates curves, in metres. The bake always works out to this distance, so it sets the length of the longest possible curve and most of the bake time. Raise it if long curves you expect are missing. |
| **Min Turn Radius** | Rejects curves whose tightest point turns sharper than this radius, in metres. `0` disables the check. |
| **Position Tube** | How close, in metres, a curve must pass to the end of a shorter curve to count as made of shorter curves. |
| **Angle Tube** | The same tolerance for the heading, in degrees. Larger tubes drop more curves and give a smaller, coarser library. |
| **Length Multiplier** | Rejects curves longer than this many times the straight distance between their ends. |

### Preview

| Field | Purpose |
|---|---|
| **Draw** | Draws the curves of one heading in the Scene view: the curves in red, and a circle of radius **Position Tube** at each curve's end. |
| **Angle Layer** | Which heading to draw, and which one **Bake Layer** bakes. |

## Key API

```csharp
void Bake()
```

Bakes the whole control set and saves it into the asset. Also available from the component's context menu.

```csharp
void BakeLayer()
```

Bakes only the heading selected in **Angle Layer**. Much faster than a full bake, which makes it useful while tuning the settings. Also available from the context menu.

```csharp
void Construct(ControlSetBakeService bakeService)
```

Replaces the bake service before the first bake. Use it to bake with a different family of curves — see [IPathBuilder](../Application/IPathBuilder.md). Without it, the baker uses clothoid curves.

| Parameter | Purpose |
|---|---|
| `bakeService` | The service to bake with. |

## Notes

- **Bake Layer overwrites the asset with that single heading.** It is a tuning tool. Run **Bake** again before you use the control set for planning.
- **Bake again when the cell size or heading count changes.** The control set is baked against the grid's cell size and the map's heading count; a set baked for different values does not fit. Moving obstacles does **not** require a new control set, because it ignores obstacles.

## Related

- [NCPFMapBuilder](NCPFMapBuilder.md) — the bake step before this one.
- [ControlSetBakeService](../Application/ControlSetBakeService.md) — the bake logic it runs.
- [FuncControlSetContainer](../../Config/FuncControlSetContainer.md) — stores the result.
- [PathLimiterBase](../../Domain/Agent/PathLimiterBase.md) — the rules that decide which curves are kept.
- [Getting Started](../../GettingStarted.md) — step 4 walks through a bake.
