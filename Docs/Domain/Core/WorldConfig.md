# WorldConfig

A pose in the world: position in metres and heading in degrees.

`NCPF.Domain` · `WorldConfig.cs`

## Description

```csharp
public struct WorldConfig
{
    float X, Y;
    float Angle;
    Vector2 Position { get; }
}
```

NCPF measures angles in **degrees**, with `0` pointing along **+Y** and positive angles turning **counter-clockwise**. A heading of `90` points along −X. This matches a Unity 2D transform whose "forward" is its up vector.

Use WorldConfig for continuous poses. For a pose on the lattice, use [Config](Config.md).

## Members

| Member | Meaning |
|---|---|
| `X`, `Y` | The position, in metres. |
| `Angle` | The heading, in degrees. |
| `Position` | `X` and `Y` as a `Vector2`. |
| `+`, `-` | Component-wise; the resulting angle is wrapped to `0..360`. |
| `*` | Scales all three components, angle included, without wrapping. |
| `Normalize(angle)` | Wraps an angle to `0..360`. |
| `DeltaAngle(a, b)` | The shortest signed difference between two angles. |

## Related

- [Config](Config.md) — a pose on the lattice.
- [DynamicState](DynamicState.md) — a pose with speed.
- [AngleUtility](AngleUtility.md) — converting between angles and directions.
