using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// Cubic curvature function κ(s) = ((A·s + B)·s + C)·s + D over an arc of
    /// length L, optionally clamped to a maximum absolute curvature.
    /// </summary>
    public class CubicPolyniomFunction : ICubicPolynomialFunction
    {
        public float A { get; }
        public float B { get; }
        public float C { get; }
        public float D { get; }
        public float MaxAbsCurvature { get; }

        public float L { get; }

        public CubicPolyniomFunction(
            float a,
            float b,
            float c,
            float d,
            float l,
            float maxAbsCurvature = float.PositiveInfinity)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            L = Mathf.Max(0.1f, l);
            MaxAbsCurvature =
                float.IsInfinity(maxAbsCurvature)
                    ? float.PositiveInfinity
                    : Mathf.Max(0f, maxAbsCurvature);
        }

        public float Evaluate(float s)
        {
            float curvature = ((A * s + B) * s + C) * s + D;

            if (float.IsPositiveInfinity(MaxAbsCurvature))
            {
                return curvature;
            }

            return Mathf.Clamp(
                curvature,
                -MaxAbsCurvature,
                MaxAbsCurvature);
        }
    }
}
