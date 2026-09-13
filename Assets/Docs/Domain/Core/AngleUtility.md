# AngleUtility

Converts between NCPF angles and direction vectors.

`NCPF.Domain` · `AngleUtility.cs`

## Description

NCPF angles are in degrees, with `0` along +Y and positive angles counter-clockwise.

## Key API

```csharp
static Vector2 AngleToDirection(float angle)
```

Returns the unit direction of `angle`: `(−sin a, cos a)`.

```csharp
static float DirectionToAngle(Vector2 dir)
```

**Do not use.** It returns the angle with the wrong sign: a direction pointing along −X, which is 90°, comes back as −90°. Nothing in the package calls it. To get the angle of a vector, use `DynamicState.VelocityAngleOf`, which is correct.

## Related

- [WorldConfig](WorldConfig.md) — the angle convention.
- [DynamicState](DynamicState.md) — `VelocityAngleOf`.
