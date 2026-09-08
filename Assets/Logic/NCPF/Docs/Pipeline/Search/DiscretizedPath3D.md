# DiscretizedPath3D

A pure geometric shape plus the lattice cells it sweeps. A decorator: all `IPath` behavior proxies to the inner path; the one thing it adds is `IdSequence`.

`NCPF.Domain · Paths/DiscretizedPath3D.cs`

## Constructor

```csharp
DiscretizedPath3D(IPath path, Config[] idSequence)
```

`path` — the shape itself ([[IPath]]), which everything proxies to.

`idSequence` — the cells the shape sweeps. Counted by [[DiscretizedControlSet3DConverter]].

## Key API

```csharp
public readonly Config[] IdSequence;
```
The one thing this class adds to [[IPath]]: the ordered list of cells the shape passes through. Read by [[ControlSetValidator3D]] for collision and by the rope agent to find where a path crosses the rope's trail.

`IdSequence` is [[Config]] with no velocity: a shape has no speed, only geometry. The cells are for **collision** ([[ControlSetValidator3D]] checks the body fits in each); the geometry (`Evaluate`) is for the search and playback.

Everything else (`Length`, `Evaluate`, `Start`, `End`, `Sample`) proxies to the inner `Path`. It also implements [[IPathWrapper]] (`Inner` → `Path`).

## Details

Built by [[DiscretizedControlSet3DConverter]], stored in layers by [[DiscretizedControlSet3D]], and carried as a graph edge by [[CurvateTransition]].

## Related

- [[IPath]] — what it decorates
- [[IPathWrapper]] — the `Inner` seam
- [[Config]] — the cell type in `IdSequence`
- [[DiscretizedControlSet3D]] — the set of such paths
- [[DiscretizedControlSet3DConverter]] — who builds them
- [[ControlSetValidator3D]] — reads `IdSequence` for collision
- [[CurvateTransition]] — carries it as an edge