# DynamicState

Full dynamic state of the body: pose ([WorldConfig](WorldConfig.md)), signed speed and the velocity heading (`VelocityAngle`).

## Details

- Speed is signed: the sign is the gear, positive nose-first, negative rear-first. The pipeline advances the cursor by |Speed| * dt.
- `Velocity` is Speed applied along `Vector2.up` rotated by VelocityAngle: `VelocityAngleOf` is its exact inverse for non-zero vectors.
- On rear-first parts the velocity heading is the tangent plus 180 degrees (the nose keeps pointing forward); see `PathGearUtility.TravelAngle`.
- The Pose part of the arithmetic operators normalizes the angle; Speed and VelocityAngle combine linearly.

## Used by

`PathAgentModel` (the runtime cursor), the planner passes (`TravelPlannerPass`, `MotionPlannerPass`) and `PathPlanner` state exchange with the game side.
