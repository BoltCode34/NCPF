# AngleUtility

Angle helpers for the NCPF convention: degrees, 0 = +Y, positive counter-clockwise.

## Details

- `AngleToDirection(a)` rotates `Vector2.up` by a degrees, giving (-sin a, cos a).
- `DirectionToAngle(dir)` is NOT the exact inverse of `AngleToDirection` (historic sign bug). Where the exact inverse matters, compute `Atan2(-dir.x, dir.y) * Rad2Deg` directly; `DynamicState.VelocityAngleOf` is the reference implementation.

## Used by

Bake geometry (tangent to heading conversions) and grid projections. Most consumers today read the convention; the helpers themselves are close to unused and are candidates for removal in a later cleanup pass.
