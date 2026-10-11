using NCPF.Domain;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    public enum FacingMode
    {
        Path = 0,
        Target = 1
    }

    /// <summary>
    /// The (sample, facing mode, speed) map over a planning window. The facing is not a free channel:
    /// at every sample the head either faces along the path (the body orientation the path was
    /// planned with) or straight at the target — there is no angle in between to rest at. A move
    /// between samples is admitted only if the speed change fits the acceleration, the speed fits the
    /// curvature, the facing change — including a switch of mode — fits the turn rate over the time
    /// the move takes, and the body stays clear while it turns and slides. So a switch is never
    /// instant: turning far means crossing slowly. While facing the target the speed is capped.
    /// The weight is the time spent NOT facing the target; time spent facing it is nearly free,
    /// which makes the plan face the target wherever that is feasible and hurry through the rest.
    /// Ending the window facing along the path also pays the time the turn back to the target would
    /// take, so the plan does not ride away from the target just because its horizon ends first.
    /// </summary>
    public class FacingModeMap : IGraph<DirectTransition>
    {
        private const float EPS = 1e-4f;
        private const int ModeCount = 2;
        private const float AimTimeWeight = 1e-3f;

        private readonly MotionWindow _window;
        private readonly IGridMap3D _map3D;
        private readonly int _speedCount, _maxSpan, _angleCount;
        private readonly float _speedStep, _aimSpeed, _wMax, _aMax, _angleStep, _startAngle;

        public FacingModeMap(MotionWindow window, IGridMap3D map3D, float startAngle, int speedCount,
                             float vMax, float aimSpeed, float wMaxDegPerSec, float aMax, int maxSpan)
        {
            _window = window;
            _map3D = map3D;
            _startAngle = startAngle;
            _speedCount = Mathf.Max(2, speedCount);
            _speedStep = Mathf.Max(0.001f, vMax) / (_speedCount - 1);
            _aimSpeed = Mathf.Max(_speedStep, aimSpeed);
            _wMax = Mathf.Max(1f, wMaxDegPerSec);
            _aMax = Mathf.Max(0.001f, aMax);
            _maxSpan = Mathf.Max(1, maxSpan);
            _angleCount = Mathf.Max(1, map3D.AngleCount);
            _angleStep = 360f / _angleCount;
        }

        public float AngleStep => _angleStep;

        public int Id(int sample, FacingMode mode, int speed)
            => (sample * ModeCount + (int)mode) * _speedCount + speed;

        public void Decode(int id, out int sample, out FacingMode mode, out int speed)
        {
            speed = id % _speedCount;
            int rest = id / _speedCount;
            mode = (FacingMode)(rest % ModeCount);
            sample = rest / ModeCount;
        }

        public float SpeedOf(int speed) => speed * _speedStep;

        public int SpeedToIndex(float speed)
            => Mathf.Clamp(Mathf.RoundToInt(Mathf.Abs(speed) / _speedStep), 0, _speedCount - 1);

        /// <summary>The facing a node stands for. The first sample is where the body already is, so
        /// both of its modes carry the body's actual facing.</summary>
        public float Facing(int sample, FacingMode mode)
        {
            if (sample == 0)
            {
                return _startAngle;
            }
            return mode == FacingMode.Target ? _window.Aim[sample] : _window.Travel[sample];
        }

        public bool PointFree(int id)
        {
            Decode(id, out int sample, out FacingMode mode, out int speed);
            if (sample < 0 || sample >= _window.Count)
            {
                return false;
            }
            if (sample > 0 && mode == FacingMode.Target && SpeedOf(speed) > _aimSpeed + EPS)
            {
                return false;
            }
            return BodyFree(sample, Facing(sample, mode));
        }

        public DirectTransition[] GetNeightbors(int id)
        {
            Decode(id, out int from, out FacingMode fromMode, out int fromSpeed);
            float speed0 = SpeedOf(fromSpeed);
            float facing0 = Facing(from, fromMode);

            List<DirectTransition> result = new List<DirectTransition>();

            for (int k = 1; k <= _maxSpan && from + k < _window.Count; k++)
            {
                int to = from + k;
                float ds = _window.Arc[to] - _window.Arc[from];
                if (ds < EPS)
                {
                    continue;
                }

                float kappa = _window.Curvature[to];

                for (int toSpeed = 0; toSpeed < _speedCount; toSpeed++)
                {
                    float speed1 = SpeedOf(toSpeed);
                    float average = 0.5f * (speed0 + speed1);
                    if (average < EPS)
                    {
                        continue;
                    }
                    if (Mathf.Abs(speed1 * speed1 - speed0 * speed0) > 2f * _aMax * ds + EPS)
                    {
                        continue;
                    }
                    if (kappa > EPS && speed1 > Mathf.Sqrt(_aMax / kappa) + 1e-3f)
                    {
                        continue;
                    }

                    float dt = ds / average;
                    float maxTurn = _wMax * dt;

                    for (int m = 0; m < ModeCount; m++)
                    {
                        FacingMode toMode = (FacingMode)m;
                        if (toMode == FacingMode.Target && speed1 > _aimSpeed + EPS)
                        {
                            continue;
                        }

                        float facing1 = Facing(to, toMode);
                        if (Mathf.Abs(Mathf.DeltaAngle(facing0, facing1)) > maxTurn)
                        {
                            continue;
                        }
                        if (!SweepFree(from, to, facing0, facing1))
                        {
                            continue;
                        }

                        bool facingTarget = fromMode == FacingMode.Target && toMode == FacingMode.Target;
                        float weight = facingTarget ? AimTimeWeight * dt : dt;
                        if (to == _window.Count - 1 && toMode == FacingMode.Path)
                        {
                            weight += Mathf.Abs(Mathf.DeltaAngle(facing1, _window.Aim[to])) / _wMax;
                        }
                        result.Add(new DirectTransition(Id(to, toMode, toSpeed), weight));
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// The body sweeps from (from, facing0) to (to, facing1): it rotates through every
        /// intermediate heading while it slides through every intermediate sample, along the shortest
        /// arc — the one the runtime takes — sub-stepped at the finer of the two resolutions.
        /// </summary>
        private bool SweepFree(int from, int to, float facing0, float facing1)
        {
            float turn = Mathf.DeltaAngle(facing0, facing1);
            int span = to - from;
            int steps = Mathf.Max(Mathf.CeilToInt(Mathf.Abs(turn) / _angleStep), span);

            for (int i = 1; i <= steps; i++)
            {
                float t = (float)i / steps;
                int sample = from + Mathf.Min(span, Mathf.RoundToInt(span * t));
                if (!BodyFree(sample, facing0 + turn * t))
                {
                    return false;
                }
            }
            return true;
        }

        private bool BodyFree(int sample, float facing)
        {
            int heading = ((Mathf.RoundToInt(facing / _angleStep) % _angleCount) + _angleCount) % _angleCount;
            return _map3D.Cell3DFree(_window.Cx[sample], _window.Cy[sample], heading);
        }
    }
}
