using NCPF.Domain;
using NCPF.Shared.Presentation;
using Core.Shared.Extentions;
using UnityEngine;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// An agent-dynamics sandbox in a vacuum — three independent pictures, all drawn by gizmos in
    /// the editor and in play mode (OnDrawGizmos): a straight-line ramp A→B coloured by speed with
    /// a time-rolling marker; an in-place spin of the body footprint around its anchor at w_max; a
    /// kinematic circle R = v/ω from the component's own knobs. Visualizes the same dynamics the
    /// planner and the simulation exercise.
    /// </summary>
    public class DynamicsVisualizer : MonoBehaviour
    {
        [Header("Wiring")]
        [SerializeField] private DynamicAgentConfig _dynamicAgent;
        [SerializeField] private AgentConfig _agentShape;

        [Header("1. Straight Ramp A->B")]
        [SerializeField] private bool _drawProfile = true;
        [SerializeField] private Transform _pointA;
        [SerializeField] private Transform _pointB;
        [SerializeField, Min(2)] private int _segments = 64;
        [SerializeField] private bool _animateMarker = true;

        [Header("2. In-Place Spin")]
        [SerializeField] private bool _drawSpin = true;

        [Header("3. Kinematic Circle")]
        [SerializeField] private bool _drawCircle = true;
        [SerializeField, Min(0f)] private float _circleSpeed = 1f;
        [SerializeField, Min(0f)] private float _circleTurnSpeed = 90f;

        private const int CircleSegments = 64;

        private void OnDrawGizmos()
        {
            if (_dynamicAgent == null)
            {
                return;
            }

            DynamicAgent agent = _dynamicAgent.Agent;
            float clock = Time.realtimeSinceStartup;

            if (_drawProfile)
            {
                DrawProfile(agent, clock);
            }
            if (_drawSpin && _agentShape != null)
            {
                DrawSpin(agent, clock);
            }
            if (_drawCircle)
            {
                DrawCircle(clock);
            }
        }

        /// <summary>Trapezoid speed profile v(s) = min(vMax, √(2a·s), √(2a·(d−s))); covers the
        /// triangle too (ramps cross below the shelf).</summary>
        private static float SpeedAt(float s, float d, float vMax, float aMax)
        {
            float rampUp = Mathf.Sqrt(2f * aMax * s);
            float rampDown = Mathf.Sqrt(2f * aMax * (d - s));
            return Mathf.Min(vMax, Mathf.Min(rampUp, rampDown));
        }

        private void DrawProfile(DynamicAgent agent, float clock)
        {
            if (_pointA == null || _pointB == null)
            {
                return;
            }

            Vector3 a = _pointA.position;
            Vector3 b = _pointB.position;
            float d = Vector3.Distance(a, b);
            if (d < 1e-3f)
            {
                return;
            }

            Vector3 dir = (b - a) / d;
            float vMax = Mathf.Max(0.001f, agent.MaxVelocity);
            float aMax = Mathf.Max(0.001f, agent.MaxLinearAcceleration);

            for (int i = 0; i < _segments; i++)
            {
                float s0 = d * i / _segments;
                float s1 = d * (i + 1) / _segments;
                float v = SpeedAt(s0, d, vMax, aMax);
                Gizmos.color = Color.Lerp(Color.red, Color.green, v / vMax);
                Gizmos.DrawLine(a + dir * s0, a + dir * s1);
            }

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(a, 0.04f);
            Gizmos.DrawSphere(b, 0.04f);

            if (_animateMarker)
            {
                DrawProfileMarker(a, dir, d, vMax, aMax, clock);
            }
        }

        /// <summary>The profile marker: a sphere rides A→B by the piecewise-analytic s(t) and loops
        /// on arrival — the dynamics IN TIME: the shelf is a long uniform stretch, the ramps are
        /// compressed accel/brake.</summary>
        private void DrawProfileMarker(Vector3 a, Vector3 dir, float d, float vMax, float aMax, float clock)
        {
            float dAcc = vMax * vMax / (2f * aMax);
            bool trapezoid = d >= 2f * dAcc;
            float vPeak = trapezoid ? vMax : Mathf.Sqrt(aMax * d);
            float t1 = vPeak / aMax;
            float T = trapezoid
                ? 2f * t1 + (d - 2f * dAcc) / vMax
                : 2f * t1;

            float tau = clock % T;
            float s;
            if (trapezoid)
            {
                if (tau < t1)
                {
                    s = 0.5f * aMax * tau * tau;
                }
                else if (tau < T - t1)
                {
                    s = dAcc + vMax * (tau - t1);
                }
                else
                {
                    float tRemain = T - tau;
                    s = d - 0.5f * aMax * tRemain * tRemain;
                }
            }
            else
            {
                if (tau < t1)
                {
                    s = 0.5f * aMax * tau * tau;
                }
                else
                {
                    float tRemain = T - tau;
                    s = d - 0.5f * aMax * tRemain * tRemain;
                }
            }

            float v = SpeedAt(s, d, vMax, aMax);
            Gizmos.color = Color.Lerp(Color.red, Color.green, v / vMax);
            Gizmos.DrawSphere(a + dir * s, 0.07f);
        }

        /// <summary>In-place spin around the body's ANCHOR at the config's w_max. The anchor→(+,+)
        /// corner diagonal makes rotation visible on a symmetric rectangle.</summary>
        private void DrawSpin(DynamicAgent agent, float clock)
        {
            float angle = (clock * agent.MaxAngledVelocity) % 360f;
            Vector3 position = transform.position;

            Gizmos.color = Color.cyan;
            Gizmos2D.DrawWireQuad(position, _agentShape.FootprintSize, angle, _agentShape.FootprintAnchor);

            Vector2 corner = new Vector2(
                1f - _agentShape.FootprintAnchor.x,
                1f - _agentShape.FootprintAnchor.y) * _agentShape.FootprintSize;
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
            Gizmos.DrawLine(position, position + rotation * corner);

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(position, 0.03f);
        }

        /// <summary>Kinematic circle of a (v, ω) pair: R = v/ω, from the component's own knobs (not
        /// the config). ω→0 — radius to infinity, no circle drawn.</summary>
        private void DrawCircle(float clock)
        {
            float w = _circleTurnSpeed * Mathf.Deg2Rad;
            if (_circleSpeed <= 1e-4f || w <= 1e-4f)
            {
                return;
            }

            float radius = _circleSpeed / w;
            Vector3 center = transform.position;

            Gizmos.color = Color.yellow;
            Vector3 prev = center + new Vector3(radius, 0f, 0f);
            for (int i = 1; i <= CircleSegments; i++)
            {
                float t = Mathf.PI * 2f * i / CircleSegments;
                Vector3 next = center + new Vector3(Mathf.Cos(t) * radius, Mathf.Sin(t) * radius, 0f);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }

            Gizmos.color = Color.white;
            Gizmos.DrawLine(center, center + new Vector3(radius, 0f, 0f));

            float phase = clock * w;
            Vector3 orbit = center + new Vector3(Mathf.Cos(phase) * radius, Mathf.Sin(phase) * radius, 0f);
            Gizmos.DrawSphere(orbit, 0.05f);
        }
    }
}
