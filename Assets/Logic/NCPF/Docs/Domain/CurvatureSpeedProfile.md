# CurvatureSpeedProfile

The speed ceiling ALONG a primitive from its actual curvature: per-segment caps min(vTop, √(a/κ), w/κ) with braking propagated backward through the segments (v[i] ≤ min(cap, √(v[i+1]² + 2·a·ds)), the far boundary being the plan's exit target magnitude).

## Details

- Where the curve is gentle the ceiling sits at vTop; toward κ peaks and the exit it descends along the √ hyperbola — braking begins exactly one braking distance away, not at the part's first tick.
- Built once per replan over the current part's remainder; the cursor only reads `Cap(arc)` (linear between probes, clamped to the nearest endpoint outside).

## Used by

The travel pass reads `Cap` while riding the current primitive.
