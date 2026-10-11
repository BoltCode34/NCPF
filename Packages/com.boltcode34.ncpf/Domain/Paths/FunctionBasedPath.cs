using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// A <see cref="FunctionBasedPath{T}"/> whose curvature is a cubic polynomial
    /// (κ(s) = A·s³ + B·s² + C·s + D). A clothoid is the degenerate cubic with
    /// A = B = 0, so this one type carries both a fitted cubic spiral and a
    /// re-integrated clothoid; exposing <see cref="IPolynomialPath"/> lets the
    /// container serialise either without knowing which it originally was.
    /// </summary>
    public class PolynomialBasedPath : FunctionBasedPath<ICubicPolynomialFunction>, IPolynomialPath
    {
        public PolynomialBasedPath(ICubicPolynomialFunction function, WorldConfig start, WorldConfig end, float minStep = 0.1f)
            : base(function, start, end, minStep)
        {
        }

        public float A => Function.A;
        public float B => Function.B;
        public float C => Function.C;
        public float D => Function.D;
    }

    /// <summary>
    /// Pure shape integrated from a curvature function κ(s): θ(s) = θ0 + ∫κ ds,
    /// p(s) = p0 + ∫dir(θ) ds. No speed, no time. The end pose is pinned to the
    /// supplied boundary at s = Length (lattice node exactness).
    /// </summary>
    public class FunctionBasedPath<T> : IPath where T : ICurvatureFunction
    {
        private const float MIN_STEP = 0.0001f;
        private const int REFINEMENT_STEPS_PER_SAMPLE = 8;

        private struct SamplePoint
        {
            public float S;
            public WorldConfig Pose;
        }

        private readonly WorldConfig _start;
        private readonly WorldConfig _end;
        private readonly float _minStep;
        private readonly float _refinementStep;

        private readonly List<SamplePoint> _samples = new();
        public readonly T Function;

        public float Length => Function.L;

        public WorldConfig Start => _start;

        public WorldConfig End => _end;

        public FunctionBasedPath(
            T function,
            WorldConfig start,
            WorldConfig end,
            float minStep = 0.1f)
        {
            Function = function;
            _start = start;
            _end = end;
            _minStep = Mathf.Max(MIN_STEP, minStep);
            _refinementStep = Mathf.Max(MIN_STEP, _minStep / REFINEMENT_STEPS_PER_SAMPLE);

            BuildSamples();
        }

        private void BuildSamples()
        {
            _samples.Clear();

            float s = 0f;
            Vector2 position = _start.Position;
            float angle = _start.Angle;

            while (s < Length)
            {
                _samples.Add(new SamplePoint
                {
                    S = s,
                    Pose = new WorldConfig(position, angle)
                });

                float ds = Mathf.Min(_minStep, Length - s);
                IntegrateSpatial(ref position, ref angle, s, ds);
                s += ds;
            }

            _samples.Add(new SamplePoint
            {
                S = Length,
                Pose = _end
            });
        }

        public WorldConfig Evaluate(float s)
        {
            if (s <= 0f)
            {
                return _start;
            }

            s = Mathf.Clamp(s, 0f, Length);

            if (s >= Length)
            {
                return _end;
            }

            SamplePoint sample = GetSampleBeforeS(s);
            return RefineByS(sample, s);
        }

        public IReadOnlyList<WorldConfig> Sample(float step)
        {
            List<WorldConfig> result = new();

            step = Mathf.Max(step, MIN_STEP);

            for (float s = 0f; s < Length; s += step)
            {
                result.Add(Evaluate(s));
            }

            result.Add(_end);

            return result;
        }

        private WorldConfig RefineByS(SamplePoint sample, float targetS)
        {
            float s = sample.S;
            Vector2 position = sample.Pose.Position;
            float angle = sample.Pose.Angle;

            while (s < targetS)
            {
                float ds = Mathf.Min(_refinementStep, targetS - s);
                IntegrateSpatial(ref position, ref angle, s, ds);
                s += ds;
            }

            return new WorldConfig(position, angle);
        }

        private void IntegrateSpatial(
            ref Vector2 position,
            ref float angle,
            float s,
            float ds)
        {
            float curvature = Function.Evaluate(s + ds * 0.5f);
            float middleAngle =
                angle
                + curvature * ds * 0.5f * Mathf.Rad2Deg;
            Vector2 direction = AngleUtility.AngleToDirection(middleAngle);

            position += direction * ds;
            angle += curvature * ds * Mathf.Rad2Deg;
        }

        private SamplePoint GetSampleBeforeS(float s)
        {
            s = Mathf.Clamp(s, 0f, Length);

            SamplePoint best = _samples[0];

            for (int i = 1; i < _samples.Count; i++)
            {
                if (_samples[i].S > s)
                {
                    break;
                }

                best = _samples[i];
            }

            return best;
        }
    }
}
