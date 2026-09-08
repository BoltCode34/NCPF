# DynamicAgent

Body limits of the moving agent: top speed (`MaxVelocity`), linear acceleration (`MaxLinearAcceleration`) and turn rate (`MaxAngledVelocity`, degrees per second).

## Details

- The agent is holonomic in heading: the nose channel is driven to the path tangent at up to `MaxAngledVelocity`, there is no non-holonomic curvature constraint on the geometry itself.
- All three limits feed the speed caps: v <= sqrt(a / k) and v <= w / k on every curve, on top of the plain speed ceiling.

## Used by

`CurvateEdgeGenerator` prices edges by these limits; `TravelPlannerPass` enforces them at runtime through the PV ladder and the local curvature clamp. In the scene the values arrive through `DynamicAgentConfig` on the presentation side.
