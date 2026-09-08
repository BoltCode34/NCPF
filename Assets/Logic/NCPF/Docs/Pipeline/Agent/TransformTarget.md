# TransformTarget

A `Transform` wrapper as an [[ITarget]]. The target is a live scene object: the pose is read straight from the transform, so it is always current.

`NCPF.Pipeline.Application · Agent/Targets.cs`

## Constructor

```csharp
TransformTarget(Transform target)
```

`target` — the scene object to follow.

## Key API

```csharp
[field: SerializeField] public Transform Transform { get; set; }
```
Serialized — so the target can be **set in the inspector** ([[PathPlanner]] keeps such a field as the default target) and swapped from there to switch to another object.

`Position` / `Rotation` / `Scale` are read from the transform on every access — no snapshot.

## Related

- [[ITarget]] — the contract
- [[PointTarget]] — if the target is just a coordinate
- [[PathPlanner]] — serialized default target
- [[ChainHead]] — wraps the transform on Follow