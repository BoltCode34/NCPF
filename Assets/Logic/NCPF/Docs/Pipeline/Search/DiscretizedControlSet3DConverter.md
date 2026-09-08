# DiscretizedControlSet3DConverter

Turns a plain shape set into a discretized one: walks each primitive by arc length and records the lattice cells it lands in. The output is a [[DiscretizedControlSet3D]].

`NCPF.Pipeline.Application · Search/DiscretizedControlSet3DConverter.cs`

## Key API

```csharp
DiscretizedControlSet3D Create(IGridGeometry3D space, ControlSet<IPath> controlSet)
```
Walks every layer and primitive of the set; for each, builds an `IdSequence` and wraps the shape in a [[DiscretizedPath3D]].

**Args**
`space` — the lattice ([[IGridGeometry3D]]); needed only to translate a world pose into a cell
`controlSet` — the source set of bare shapes ([[ControlSet]] of [[IPath]])

**Return** [[DiscretizedControlSet3D]] — the same set, but every shape knows its cells

```csharp
public float Step = 0.05f;
```
The arc-length sampling step. Finer = more accurate cell list, but slower and more memory.

## BuildCells

Steps the path from `0` to `Length` in `Step` increments, converts each pose to a cell (`space.WorldToCell3D`), and adds it to the list — **skipping consecutive repeats**, so the result is the ordered sequence of distinct cells the shape passes through. No time, no velocity: an [[IPath]] is parameterised by **length**, so this is literally "step along the shape and write down the cells."

The end is clamped to `Length`: `samples · Step` overshoots when the length is not a multiple of the step, and evaluating the shape past its end is invalid.

## Why IGridGeometry3D

Only the **geometry** is needed — world pose to cell. No search, no velocity, nothing else.

## Related

- [[ControlSet]] — the input (`ControlSet<IPath>`)
- [[DiscretizedControlSet3D]] — the output
- [[DiscretizedPath3D]] — what each primitive is wrapped into
- [[IGridGeometry3D]] — the lattice for `WorldToCell3D`