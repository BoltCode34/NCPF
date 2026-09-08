using NCPF.Pipeline.Application;
using NCPF.Domain;
using System;
using UnityEngine;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// The travel main planner: ride the chosen geometry at a SEARCHED speed. The path is pure
    /// geometry; this pass layers the 4th dimension — a short-horizon search over the primitive
    /// chain under the cursor, admitting only dynamically feasible speed transitions (tangential
    /// |Δv²| ≤ 2·a·L, centripetal v ≤ √(a/κ_peak), nose-tracking v ≤ w_max/κ_peak). The ladder is
    /// SIGNED: the sign is the gear (+ nose-first, − rear-first); a gear change passes through a
    /// full stop. Facing follows the geometry; two preferences: the ladder top
    /// (<c>_targetSpeedFactor</c>) and the striving strength toward it (<c>_speedStrive</c>).
    /// </summary>
    [Serializable]
    public class TravelPlannerFeature : MainPlannerFeature
    {
        [Header("Search Window")]
        [SerializeField, Range(0f, 1f)] private float _targetSpeedFactor = 1f;
        [SerializeField, Min(0f)] private float _speedStrive = 0f;
        [SerializeField] private float _lookahead = 3f;
        [SerializeField] private int _speedSteps = 5;
        [SerializeField] private float _planInterval = 0.1f;
        [SerializeField] private int _maxIterations = 1024;

        public override MainSAPass CreatePass(PathAgentContext context)
        {
            DynamicAgent a = context.DynamicAgent;
            return new TravelPlannerPass(
                context.Model,
                a.MaxVelocity * Mathf.Clamp01(_targetSpeedFactor),
                a.MaxVelocity, a.MaxLinearAcceleration, a.MaxAngledVelocity,
                _lookahead, _speedSteps, _planInterval, _maxIterations, _speedStrive);
        }

        /// <summary>
        /// The main pass body: drives a signed speed along the path by a 1D search over primitives,
        /// and the facing along the geometry. Caches the plan between replans and chases it every
        /// tick within the real dynamic limits.
        /// </summary>
        public class TravelPlannerPass : MainSAPass
        {
            private readonly PathAgentModel _model;
            private readonly float _vTop, _vMax, _aMax, _wMax, _lookahead, _planInterval;
            private readonly int _speedSteps, _maxIters;
            private readonly float _speedStrive;
            private readonly GoalPathFinderBase<DirectTransition> _finder = new GoalPathFinderBase<DirectTransition>();

            private float _lastPlanTime = float.NegativeInfinity;
            private float _planSpeed;
            private bool _hasPlan;

            private CurvatureSpeedProfile _profile;

            private float _clock;

            public TravelPlannerPass(PathAgentModel model, float vTop, float vMax, float aMax,
                                     float wMaxDegPerSec, float lookahead, int speedSteps,
                                     float planInterval, int maxIterations, float speedStrive = 0f)
            {
                _model = model;
                _vTop = Mathf.Max(0.001f, vTop);
                _vMax = Mathf.Max(0.001f, vMax);
                _aMax = Mathf.Max(0.001f, aMax);
                _wMax = Mathf.Max(1f, wMaxDegPerSec);
                _lookahead = Mathf.Max(0.2f, lookahead);
                _speedSteps = Mathf.Max(1, speedSteps);
                _planInterval = Mathf.Max(0f, planInterval);
                _maxIters = Mathf.Max(1, maxIterations);
                _speedStrive = Mathf.Max(0f, speedStrive);
            }

            public override ControlInput PlanControl(float dt, DynamicState current)
            {
                if (_model.CurrentPart == null)
                {
                    return new ControlInput(0f, current.Angle);
                }

                _clock += dt;

                if (_clock - _lastPlanTime >= _planInterval || !_hasPlan)
                {
                    _lastPlanTime = _clock;
                    Replan(current);
                    _hasPlan = true;
                }

                float capHere = _profile != null
                    ? _profile.Cap(_model.CurrentPartArc)
                    : Mathf.Abs(_planSpeed);
                float target = PathGearUtility.GearOf(_model.CurrentPart) * capHere;
                float speed = Mathf.MoveTowards(current.Speed, target, _aMax * dt);

                speed = ClampByLocalCap(speed);

                float tangent = _model.CurrentPart.Evaluate(_model.CurrentPartArc).Angle;
                float facing = Mathf.MoveTowardsAngle(current.Angle, tangent, _wMax * dt);

                return new ControlInput(speed, facing);
            }

            /// <summary>Clamps |speed| by the κ at the cursor: cap = min(vTop, √(a/κ), w_max/κ) —
            /// all three DynamicAgent limits from the LOCAL curvature. The sign survives.</summary>
            private float ClampByLocalCap(float speed)
            {
                float kappa = PrimitiveWindow.KappaAt(_model.CurrentPart, _model.CurrentPartArc);
                if (kappa <= 1e-6f)
                {
                    return Mathf.Clamp(speed, -_vTop, _vTop);
                }

                float wMaxRad = _wMax * Mathf.Deg2Rad;
                float cap = Mathf.Min(_vTop, Mathf.Min(Mathf.Sqrt(_aMax / kappa), wMaxRad / kappa));
                return Mathf.Clamp(speed, -cap, cap);
            }

            /// <summary>The expensive part: builds the primitive window + PV-map + search. Writes
            /// the result into <see cref="_planSpeed"/>. Throttled, not every tick.</summary>
            private void Replan(DynamicState current)
            {
                _planSpeed = 0f;

                PrimitiveWindow window = new PrimitiveWindow(_model, _lookahead, _vTop, _aMax, _wMax);
                if (window.Count == 0)
                {
                    _profile = null;
                    return;
                }

                SpacedMapPV map = new SpacedMapPV(window, _vTop, _aMax, _speedSteps, _speedStrive);

                IGoal goal = new GoalBase(
                    new PrimitiveEndAchivement(map),
                    new PrimitiveTimeHeuristic(window, map, _vMax));

                int b0 = map.SpeedToBucket(current.Speed);
                int gear = PathGearUtility.GearOf(_model.CurrentPart);
                if (b0 * gear < 0)
                {
                    b0 = -b0;
                }

                DirectTransition[] chain = _finder.FindPath(
                    map, map.CellPVToId(new ConfigPV(0, b0)), goal, _maxIters);

                if (chain != null && chain.Length > 0)
                {
                    _planSpeed = map.BucketSpeed(map.IdToCellPV(chain[0].Neightbor).Speed);
                }
                else if (b0 == 0)
                {
                    _planSpeed = window.Parts[0].Gear * Mathf.Sqrt(_aMax * window.Parts[0].Length * 0.5f);
                }

                _profile = CurvatureSpeedProfile.Build(
                    _model.CurrentPart, _model.CurrentPartArc,
                    _vTop, _aMax, _wMax, Mathf.Abs(_planSpeed));
            }
        }
    }
}
