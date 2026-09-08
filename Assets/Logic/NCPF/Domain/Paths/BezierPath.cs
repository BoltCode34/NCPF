using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// Cubic Bezier curve between two poses: start and end positions come from
    /// the boundary poses, the two middle control points are free. Length is
    /// approximated by chord sampling; Evaluate maps arc length to the curve
    /// parameter through that approximation.
    /// </summary>
    public class BezierPath : IPath
    {
        private readonly Vector2 _p0;
        private readonly Vector2 _p1;
        private readonly Vector2 _p2;
        private readonly Vector2 _p3;

        public float Length { get; }

        public WorldConfig Start { get; }

        public WorldConfig End { get; }

        public BezierPath(
            WorldConfig start,
            Vector2 p1,
            Vector2 p2,
            WorldConfig end)
        {
            Start = start;
            End = end;

            _p0 = start.Position;
            _p1 = p1;
            _p2 = p2;
            _p3 = end.Position;

            Length = ApproximateLength();
        }

        public WorldConfig Evaluate(float s)
        {
            if (Length <= 0.0001f)
                return Start;

            float t =
                Mathf.Clamp01(s / Length);

            Vector2 position =
                EvaluatePoint(t);

            Vector2 tangent =
                EvaluateTangent(t);

            float angle =
                DirectionToAngle(tangent);

            return new WorldConfig(
                position.x,
                position.y,
                angle);
        }

        public IReadOnlyList<WorldConfig> Sample(float step)
        {
            List<WorldConfig> samples = new();

            if (step <= 0f)
                step = 0.1f;

            for (float s = 0; s < Length; s += step)
            {
                samples.Add(Evaluate(s));
            }

            samples.Add(Evaluate(Length));

            return samples;
        }

        private Vector2 EvaluatePoint(float t)
        {
            float u = 1f - t;

            return
                u * u * u * _p0 +
                3f * u * u * t * _p1 +
                3f * u * t * t * _p2 +
                t * t * t * _p3;
        }

        private Vector2 EvaluateTangent(float t)
        {
            float u = 1f - t;

            Vector2 tangent =
                3f * u * u * (_p1 - _p0) +
                6f * u * t * (_p2 - _p1) +
                3f * t * t * (_p3 - _p2);

            if (tangent.sqrMagnitude < 0.0001f)
                return Vector2.up;

            return tangent.normalized;
        }

        private float DirectionToAngle(Vector2 dir)
        {
            if (dir.sqrMagnitude < 0.00001f)
                return 0f;

            float angle =
                Vector2.SignedAngle(
                    Vector2.up,
                    dir.normalized);

            if (angle < 0f)
                angle += 360f;

            return angle;
        }

        private float ApproximateLength()
        {
            const int samples = 100;

            float length = 0f;

            Vector2 prev = _p0;

            for (int i = 1; i <= samples; i++)
            {
                float t =
                    i / (float)samples;

                Vector2 current =
                    EvaluatePoint(t);

                length +=
                    Vector2.Distance(
                        prev,
                        current);

                prev = current;
            }

            return length;
        }
    }
}
