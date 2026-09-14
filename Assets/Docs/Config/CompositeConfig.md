# CompositeConfig

A body shape assembled from primitive shapes in the Inspector, for bodies no single primitive describes — for example an I-beam made of a web and two flanges.

`NCPF.Config` · `CompositeConfig.cs`

## Description

Create one with **Create > NCPF > CompositeConfig**. It implements the [AgentConfig](../Shared/AgentConfig.md) port and creates a [CompositeAgent](../Domain/Collision/CompositeAgent.md).

The config keeps a list of **parts**. For each part you pick a shape type from a dropdown, set its dimensions, and place it inside the composite with an offset and a rotation. The composite's bounding rectangle is the union of the parts, and its anchor is a fraction of that union.

The Inspector preview shows the assembled body as it will be baked. See [AgentConfig](../Shared/AgentConfig.md#inspector-preview).

## Inspector

| Field | Purpose |
|---|---|
| **Parts** | The shapes the body is made of. |
| **Anchor** | The point the body is moved by, as a fraction of the bounding rectangle of all parts: `(0.5, 0.5)` is its centre, `(0.5, 1)` the middle of its front edge. |

Each part:

| Field | Purpose |
|---|---|
| **Offset** | The centre of the shape inside the composite, in metres: `x` across the heading, `y` along it. |
| **Rotation** | The rotation of the shape inside the composite, in degrees, counter-clockwise. |
| **Shape** | The shape. Pick a type from the dropdown, then set its fields. The dropdown lists every serializable [Agent](../Domain/Collision/Agent.md) with a parameterless constructor, including your own. |

## Example: an I-beam

The `IBeamConfig` asset in `Assets/Materials/Configs/IBeam`, used by the `IBeamAgent` sample scene, has the heading along the web and flanges of different widths:

| Part | Shape | Offset | Rotation |
|---|---|---|---|
| Web | Rectangle, Width 0.18, Height 3 | `(0, 0)` | 0 |
| Front flange | Rectangle, Width 1.5, Height 0.18 | `(0, 1.5)` | 0 |
| Back flange | Rectangle, Width 3, Height 0.18 | `(0, -1.5)` | 0 |

Anchor `(0.5, 0.5)`: the body is moved by its centre.

## Notes

- **Only the relative placement of parts matters.** The composite is centred on the union of its parts, so moving every part by the same offset changes nothing.
- **Composites cannot be nested.** A composite is not offered in the Shape dropdown.
- **The anchor must match your agent's pivot,** and the map must be baked again after changing any part. See [RectangularConfig](RectangularConfig.md#notes).
- The body gizmo on [PathPlanner](../Pipeline/Presentation/PathPlanner.md) is refreshed when the composite's bounding rectangle or anchor changes. A change inside that keeps both the same shows up once the planner is re-enabled, for example after entering Play mode or a script reload.

## Related

- [CompositeAgent](../Domain/Collision/CompositeAgent.md) — the shape it creates.
- [Agent](../Domain/Collision/Agent.md) — the primitive shapes.
- [Shared editors](../Shared/Editor/SharedEditors.md) — the shape dropdown.
