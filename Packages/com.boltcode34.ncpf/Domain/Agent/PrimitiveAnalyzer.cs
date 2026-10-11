using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// The analysis procedure: shape → <see cref="PrimitiveAnalysis"/>. The
    /// single point of truth for what analysis means — the bake tool calls the
    /// same procedure, so baked and computed never drift apart. Polynomial
    /// paths (<see cref="IPolynomialPath"/>: a clothoid is the degenerate
    /// cubic A=B=0) are analyzed ANALYTICALLY: the coefficients ARE κ(s), the
    /// peak of |κ| over the span is a closed formula. A black-box
    /// <see cref="IPath"/> falls back to tangent-turn sampling.
    /// </summary>
    public static class PrimitiveAnalyzer
    {
        private const float POLY_EPS = 1e-6f;

        /// <summary>The probe step for black-box sampling, m.</summary>
        public const float ProbeStep = 0.05f;

        public static PrimitiveAnalysis Analyze(IPath path)
        {
            return path is IPolynomialPath polynomial
                ? new PrimitiveAnalysis(KappaPeakPolynomial(polynomial))
                : new PrimitiveAnalysis(KappaPeakSampled(path));
        }

        /// <summary>
        /// κ(s) = ((A·s + B)·s + C)·s + D, coefficients in rad/m. |κ| over
        /// [0, L] peaks at the ends or at roots of κ′(s) = 3A·s² + 2B·s + C = 0
        /// inside the span; roots outside [0, L] are skipped — their values
        /// belong to a span the path never travels and could inflate the peak.
        /// </summary>
        private static float KappaPeakPolynomial(IPolynomialPath path)
        {
            float l = path.Length;
            float peak = Mathf.Max(Mathf.Abs(path.D), Mathf.Abs(KappaOf(path, l)));

            if (Mathf.Abs(path.A) < POLY_EPS && Mathf.Abs(path.B) < POLY_EPS)
            {
                return peak;
            }

            float discriminant = path.B * path.B - 3f * path.A * path.C;
            if (discriminant <= 0f)
            {
                return peak;
            }

            float root = Mathf.Sqrt(discriminant);
            peak = MaxInsideSpan(path, peak, (-path.B - root) / (3f * path.A), l);
            peak = MaxInsideSpan(path, peak, (-path.B + root) / (3f * path.A), l);
            return peak;
        }

        private static float MaxInsideSpan(IPolynomialPath path, float peak, float s, float l)
        {
            if (s <= 0f || s >= l)
            {
                return peak;
            }
            return Mathf.Max(peak, Mathf.Abs(KappaOf(path, s)));
        }

        private static float KappaOf(IPolynomialPath path, float s)
        {
            return ((path.A * s + path.B) * s + path.C) * s + path.D;
        }

        /// <summary>
        /// Black box: κ as the tangent turn between probes. An approximation:
        /// a narrow spike between probes can be underestimated (the polynomial
        /// branch has no such error at all).
        /// </summary>
        private static float KappaPeakSampled(IPath path)
        {
            float length = path.Length;
            int samples = Mathf.Clamp(Mathf.CeilToInt(length / ProbeStep), 4, 32);
            float ds = length / samples;

            float prev = path.Evaluate(0f).Angle;
            float peak = 0f;
            for (int i = 1; i <= samples; i++)
            {
                float angle = path.Evaluate(i * ds).Angle;
                float kappa = Mathf.Abs(Mathf.DeltaAngle(prev, angle)) * Mathf.Deg2Rad / ds;
                if (kappa > peak)
                {
                    peak = kappa;
                }
                prev = angle;
            }
            return peak;
        }
    }
}
