# PathGearUtility

The gear of a primitive: +1 nose-first (tangent = heading), −1 rear-first (tangent = heading + 180°).

## Details

- A rear-first primitive carries the `IRearFirstPath` marker, but it can be wrapped at any depth by decorators, so `IsRearFirst` looks through `IPathWrapper` wrappers recursively.
- `TravelAngle` is the motion tangent at an arc — the nose channel and the tangent channel are different things, and the body moves along the tangent.

## Used by

The follower (seed gear check on cold start), the window/profile builders (part gear) and the PV ladder (signed speed buckets).
