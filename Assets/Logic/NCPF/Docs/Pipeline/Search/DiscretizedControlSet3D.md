# DiscretizedControlSet3D

A control set of [[DiscretizedPath3D]] entries: every primitive carries its lattice cell sweep alongside its geometry. The discretized counterpart of [[ControlSet]] — same layering by start heading, but the elements know the cells they pass through instead of being bare [[IPath]].

`NCPF.Domain · ControlSet/DiscretizedControlSet3D.cs`

## What it is

```csharp
public class DiscretizedControlSet3D : ControlSet<DiscretizedPath3D> { }
```

A thin specialization of [[ControlSet]]. Like any control set, it stores primitives **layered by start direction** (see [[ControlSet]]).

Built from a plain `ControlSet<IPath>` by [[DiscretizedControlSet3DConverter]]. Consumed by [[Projector3D]], which projects these primitives onto graph nodes; the [[CurvateEdgeGenerator]] reads the library through the [[PathOffset]] wrapper to price survivors by reference.

## Related

- [[ControlSet]] — the base set
- [[DiscretizedPath3D]] — the element
- [[DiscretizedControlSet3DConverter]] — how to get it from `ControlSet<IPath>`
- [[Projector3D]] — consumer
- [[CurvateEdgeGenerator]] — prices its entries