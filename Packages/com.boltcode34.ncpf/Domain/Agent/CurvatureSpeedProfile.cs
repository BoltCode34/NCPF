using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// The speed ceiling ALONG a primitive from its actual curvature. κ is
    /// sampled as the heading turn over arc; every segment yields its cap
    /// min(vTop, √(a/κ), w/κ), then braking is propagated BACKWARD through the
    /// segments: v[i] ≤ min(segment cap, √(v[i+1]² + 2·a·ds)); the far boundary
    /// is the plan's exit target magnitude. Where the curve is gentle the
    /// ceiling sits at vTop; toward κ peaks and the exit it descends along the
    /// √ hyperbola — braking begins exactly one braking distance away.
    /// </summary>
    public class CurvatureSpeedProfile
    {
        private const float KappaEps = 1e-6f;

        private readonly float _startArc;
        private readonly float _ds;
        private readonly float[] _caps;

        private CurvatureSpeedProfile(float startArc, float ds, float[] caps)
        {
            _startArc = startArc;
            _ds = ds;
            _caps = caps;
        }

        /// <summary>
        /// The profile over the part's remainder from startArc to its end;
        /// exitSpeed is the plan's exit target MAGNITUDE (the backward pass
        /// boundary). A degenerate remainder yields the constant exitSpeed.
        /// </summary>
        public static CurvatureSpeedProfile Build(IPath part, float startArc,
                                                  float vTop, float aMax,
                                                  float wMaxDegPerSec, float exitSpeed)
        {
            vTop = Mathf.Max(0.001f, vTop);
            aMax = Mathf.Max(0.001f, aMax);
            float wMaxRad = Mathf.Max(1f, wMaxDegPerSec) * Mathf.Deg2Rad;
            float exit = Mathf.Clamp(Mathf.Abs(exitSpeed), 0f, vTop);

            float length = part.Length - startArc;
            if (length <= PrimitiveWindow.MinPartLength)
            {
                return new CurvatureSpeedProfile(startArc, 0f, new[] { exit });
            }

            int samples = Mathf.Clamp(Mathf.CeilToInt(length / 0.05f), 8, 64);
            float ds = length / samples;

            float[] caps = new float[samples + 1];
            float prev = part.Evaluate(startArc).Angle;
            for (int i = 0; i < samples; i++)
            {
                float angle = part.Evaluate(startArc + (i + 1) * ds).Angle;
                float kappa = Mathf.Abs(Mathf.DeltaAngle(prev, angle)) * Mathf.Deg2Rad / ds;
                prev = angle;

                caps[i] = kappa > KappaEps
                    ? Mathf.Min(vTop, Mathf.Min(Mathf.Sqrt(aMax / kappa), wMaxRad / kappa))
                    : vTop;
            }
            caps[samples] = exit;

            for (int i = samples - 1; i >= 0; i--)
            {
                float brake = Mathf.Sqrt(caps[i + 1] * caps[i + 1] + 2f * aMax * ds);
                if (brake < caps[i])
                {
                    caps[i] = brake;
                }
            }

            return new CurvatureSpeedProfile(startArc, ds, caps);
        }

        /// <summary>
        /// The ceiling at arc — a MAGNITUDE, m/s. Outside the remainder — the
        /// nearest point: before the entry, past the end the exit target.
        /// Linear between probes.
        /// </summary>
        public float Cap(float arc)
        {
            if (_ds <= 0f || _caps.Length == 1)
            {
                return _caps[0];
            }

            float t = Mathf.Clamp((arc - _startArc) / _ds, 0f, _caps.Length - 1);
            int i = Mathf.Min((int)t, _caps.Length - 2);
            return Mathf.Lerp(_caps[i], _caps[i + 1], t - i);
        }
    }
}
