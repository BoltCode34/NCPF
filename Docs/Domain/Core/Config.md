# Config

A pose on the lattice: a cell and a heading index.

`NCPF.Domain` · `Config.cs`

## Description

```csharp
public struct Config
{
    int X, Y;
    int Angle;
    Int2 Cell { get; }
}
```

`X` and `Y` are cell coordinates. `Angle` is the **index** of a heading, not degrees: with 16 headings, index `4` is 90°. The grid converts between Config and [WorldConfig](WorldConfig.md).

Configs are also used as offsets, for example the cells a curve passes through relative to its start, which is why they support arithmetic.

## Members

| Member | Meaning |
|---|---|
| `X`, `Y` | The cell. |
| `Angle` | The heading index. |
| `Cell` | `X` and `Y` as an `Int2`. |
| `+`, `-`, `*`, `/` | Component-wise, including `Angle`. |
| `==`, `!=` | All three components equal. |

## Notes

- **Arithmetic does not wrap the heading.** Adding two Configs can produce an `Angle` below zero or at or above the heading count. Wrap it yourself before using it as a heading.

## Related

- [WorldConfig](WorldConfig.md) — a continuous pose.
- [IGridMap3D](../Space/IGridMap3D.md) — converts between the two.
