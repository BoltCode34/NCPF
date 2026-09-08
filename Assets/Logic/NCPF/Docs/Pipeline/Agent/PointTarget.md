# PointTarget

A bare point as an [[ITarget]]. It doesn't follow anything — it just stands where it was placed.

`NCPF.Pipeline.Application · Agent/Targets.cs`

## Constructor

```csharp
PointTarget(Vector2 position)
```

`position` — where the target stands. `Rotation` and `Scale` get neutral values (`identity` / `one`).

## Why

For states that aim at a **computed place**, not an object: an orbital point around the prey, the last known position, a patrol point. Such a target has nothing to follow — it is just a coordinate.

Used in [[ChainHead]] on `Follow(Vector2)`:

```csharp
protected override void OnStartFollowPoint(Vector2 target)
    => Planner.Target = new PointTarget(target);
```

Position is publicly mutable (`{ get; set; }`) — the point can be moved without recreating the target.

## Related

- [[ITarget]] — the contract
- [[TransformTarget]] — if the target is a scene object
- [[ChainHead]] — wraps the point on Follow