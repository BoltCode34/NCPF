# IPath

A pure trajectory: pose (position and direction of travel) as a function of arc length. The only path contract in the domain.

## Details

- `Length` is the total arc in metres; `Evaluate(s)` returns the pose at arc s; `Sample(step)` returns evenly spaced poses.
- Speed and time are not part of the contract. Timing lives in the pipeline layers (`TravelPlannerPass` and its PV speed ladder).
- `Start` and `End` are the boundary poses.

# IPolynomialPath

A path whose curvature is the cubic polynomial k(s) = A*s^3 + B*s^2 + C*s + D.

## Details

- A clothoid is the degenerate cubic (A = B = 0), so clothoids and general cubics serialize through the same (A, B, C, D, L) store; see `FuncControlSetContainer`.
- The polynomial coefficients are curvature in rad/m by arc, which is what `PrimitiveAnalyzer` reads for speed caps.

## Used by

Bake produces `IPolynomialPath` implementations (clothoid and cubic builders); the pipeline consumes `IPath` for geometry and `IPolynomialPath` for curvature analysis.
