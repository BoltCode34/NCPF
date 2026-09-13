# PathLimiterBase

The rules the control set bake uses to decide whether a candidate curve is kept.

`NCPF.Domain` · `PathLimiterBase.cs`

## Description

The bake produces many candidate curves. A useful library keeps only curves that add a genuinely new move. PathLimiterBase rejects a candidate if any of these hold:

1. **Too roundabout.** Its length is more than `breakDistance` times the straight distance between its ends.
2. **Too sharp.** Its tightest turning radius is below `minTurnRadius`.
3. **Made of shorter curves.** Somewhere between its ends it passes a lattice pose the library already reaches, close enough in position and heading. Such a curve can be replaced by two shorter ones.

For rule 3, a pose counts as passed when a sample of the curve lies within `positionTube` metres of that pose's cell centre and within `angleTube` degrees of its heading. The curve is sampled densely enough, relative to the tube, that a long gentle curve cannot slip past a pose between two samples. Its own start and end cells are ignored.

Feasibility at speed is not checked here: whether a curve can be taken fast is decided during the search.

## IPathLimiter

```csharp
public interface IPathLimiter
{
    bool TryAllowPath(IPath path, IGridGeometry3D gridSpace, HashSet<Config> states);
    bool CheckLength(IPath path);
    bool CanBeDivided(IGridGeometry3D space, HashSet<Config> states, IPath path);
}
```

Implement it, or derive from PathLimiterBase and override its virtual methods, to use your own rules.

## Constructor

```csharp
PathLimiterBase(float breakDistance = 1.4f, float positionTube = 0.25f, float angleTube = 10f, float minTurnRadius = 0f)
```

| Parameter | Purpose |
|---|---|
| `breakDistance` | The allowed ratio of length to straight distance. |
| `positionTube` | The position tolerance for rule 3, in metres. |
| `angleTube` | The heading tolerance for rule 3, in degrees. |
| `minTurnRadius` | The smallest allowed turning radius, in metres. `0` disables rule 2. |

## Key API

```csharp
bool TryAllowPath(IPath path, IGridGeometry3D gridSpace, HashSet<Config> states)
```

Returns `true` if the curve passes all three rules.

| Parameter | Purpose |
|---|---|
| `path` | The candidate. |
| `gridSpace` | Converts samples to lattice poses. |
| `states` | The lattice poses the library already reaches. |

## Related

- [ControlSetBaker](../../Bake/Presentation/ControlSetBaker.md) — sets these values in the Inspector.
- [LayerControlSetBuilderBase](../../Bake/Application/LayerControlSetBuilderBase.md) — applies the rules.
