# PathOffset

Translates a shape by a world offset, leaving its direction untouched. It puts a primitive authored at the origin onto a node's position — a pure translation.

`NCPF.Pipeline.Application · Space/PathOffset.cs`

## What it is

A decorator over [[IPath]]: every `Evaluate`/`Start`/`End`/`Sample` proxies to the inner path with `shift` added to the position; `Angle` is unchanged. It also implements [[IPathWrapper]], exposing the raw library entry as `Inner` — so the [[CurvateEdgeGenerator]] can identify a survivor by **reference** and price it without re-analysis.

```csharp
public sealed class PathOffset : IPath, IPathWrapper
{
    public PathOffset(IPath inner, Vector2 shift);
    public IPath Inner { get; }
    // Evaluate/Start/End/Sample — add shift to position, keep Angle
}
```

## Why

The primitive library ([[DiscretizedControlSet3D]]) is authored at the origin, already rotated by layer. Placing a shape at a node is a pure translation by `(nodePos − origin)`. [[Projector3D]] uses this in `Expand`.

Because translation leaves curvature intact, the generator can cache the curvature analysis on the library entry (`Inner`) and still recognize the same shape after it has been offset — the wrapper carries the identity.

## Related

- [[IPath]] — what it decorates
- [[IPathWrapper]] — the `Inner` seam
- [[Projector3D]] — uses it to shift a shape onto a node
- [[CurvateEdgeGenerator]] — reads `Inner` to price by reference