# PrimitiveAnalyzer

The analysis procedure: shape → `PrimitiveAnalysis` (the peak of |κ| over the span). The single point of truth for what analysis means — the bake tool calls the same procedure, so baked and computed never drift apart.

## Details

- Polynomial paths are analyzed ANALYTICALLY: the coefficients ARE κ(s), the peak of |κ| over the span is a closed formula (ends plus roots of κ′ inside the interval).
- A black-box `IPath` falls back to tangent-turn sampling — an approximation; a narrow spike between probes can be underestimated.
- The analysis is projection-invariant (translation, rotation, gear), so one analysis answers one library entry, not each projected instance. Dynamics are not part of it: they belong to the weight formula.

## Used by

Edge pricing uses the analysis for curvature caps and penalties.
