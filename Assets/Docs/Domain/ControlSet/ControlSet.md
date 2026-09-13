# ControlSet

The library of curves an agent may drive, grouped by the heading each curve starts from.

`NCPF.Domain` · `ControlSet.cs`

## Description

A control set is the core idea of a state lattice. Instead of letting the agent turn freely, the search may only chain together curves from a fixed library. Each curve starts at a lattice node and ends exactly on another, so any chain of them is a smooth drivable path.

All curves are stored as they start from the origin. A **layer** holds the curves that start at one heading index; the search uses the layer matching a node's heading and moves the curves to the node.

### Ids

Each curve also has a single integer **id**, so tables and search edges can refer to it without holding the curve itself. The set is the only thing that knows how ids are formed: always convert through `IdOf`, `LayerOf`, `IndexOf` and `GetById`, and never take an id apart yourself.

Ids are dense, from `0` to `IdCount − 1`, so a table indexed by id can be a plain array.

## Key API

```csharp
void LoadLayer(int angle, params T[] paths)
T[] GetLayerSet(int angle)
int[] GetLayers()
```

Stores the curves of one heading index, returns them (or `null` if the layer is missing), and lists the loaded heading indices in order.

```csharp
int IdOf(int layer, int index)
int LayerOf(int id)
int IndexOf(int id)
T GetById(int id)
```

Converts between a curve's position in its layer and its id. `GetById` returns `default` for an unknown id.

| Property | Meaning |
|---|---|
| `Count` | The number of loaded layers. |
| `AngleCount` | The highest loaded heading index plus one. |
| `Stride` | The size of the largest layer; ids reserve this many slots per layer. |
| `IdCount` | `AngleCount × Stride`. |

## Notes

- **Load every layer before creating ids.** `Stride` grows when a larger layer is loaded, which changes every id. Tables built from ids before that are wrong afterwards.

## Related

- [DiscretizedControlSet3D](DiscretizedControlSet3D.md) — the set the search uses.
- [ControlSetBaker](../../Bake/Presentation/ControlSetBaker.md) — creates control sets.
- [QuarterSymmetry](QuarterSymmetry.md) — why only a quarter is baked.
