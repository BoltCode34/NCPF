using NCPF.Pipeline.Application;
using NCPF.Domain;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// The look-at-target main planner: the head faces the target wherever that is feasible and
    /// faces along the path everywhere else — never somewhere in between. Facing and speed are
    /// planned together over the look-ahead window, because the turn affordable on a stretch depends
    /// on how slowly it is crossed: facing the target caps the speed, facing along the path lets the
    /// agent hurry to cut the time it spends not facing the target, and a switch between the two
    /// takes as long as the turn rate demands.
    /// </summary>
    [Serializable]
    public class LookAtTargetFeature : MainPlannerFeature
    {
        [Header("Planning Window")]
        [SerializeField] private float _lookahead = 3f;
        [SerializeField] private float _sampleStep = 0.15f;
        [SerializeField] private int _speedCount = 5;
        [SerializeField] private int _maxSpan = 3;
        [SerializeField] private int _maxIterations = 4000;
        [SerializeField] private float _planInterval = 0.1f;

        [Header("Facing")]
        [SerializeField, Range(0f, 1f)] private float _aimSpeedFactor = 0.25f;

        public override MainSAPass CreatePass(PathAgentContext context)
        {
            DynamicAgent a = context.DynamicAgent;
            return new LookAtTargetPass(
                context.Model, context.Grid, context.Target,
                new GoalPathFinderBase<DirectTransition>(),
                a.MaxAngledVelocity, a.MaxLinearAcceleration, a.MaxVelocity,
                a.MaxVelocity * Mathf.Clamp01(_aimSpeedFactor),
                _lookahead, _sampleStep, _speedCount, _maxSpan, _maxIterations, _planInterval);
        }

        /// <summary>
        /// The main pass body: searches (facing mode, speed) over the look-ahead window, then chases
        /// the chosen mode and speed every tick within the real dynamic limits. The mode is kept, not
        /// the angle: facing the target tracks the live bearing, facing the path tracks the live
        /// tangent under the cursor.
        /// </summary>
        public class LookAtTargetPass : MainSAPass
        {
            private readonly PathAgentModel _model;
            private readonly IGridMap3D _grid;
            private readonly ITarget _target;
            private readonly IGoalPathFinder<DirectTransition> _finder;
            private readonly float _wMax, _aMax, _vMax, _aimSpeed, _lookahead, _sampleStep, _planInterval;
            private readonly int _speedCount, _maxSpan, _maxIters;

            private float _clock;
            private float _lastPlanTime = float.NegativeInfinity;
            private FacingMode _planMode = FacingMode.Path;
            private float _planSpeed;
            private bool _hasPlan;

            public LookAtTargetPass(PathAgentModel model, IGridMap3D grid, ITarget target,
                                    IGoalPathFinder<DirectTransition> finder,
                                    float wMax, float aMax, float vMax, float aimSpeed,
                                    float lookahead, float sampleStep, int speedCount, int maxSpan,
                                    int maxIters, float planInterval)
            {
                _model = model;
                _grid = grid;
                _target = target;
                _finder = finder;
                _wMax = Mathf.Max(1f, wMax);
                _aMax = Mathf.Max(0.001f, aMax);
                _vMax = Mathf.Max(0.001f, vMax);
                _aimSpeed = Mathf.Clamp(aimSpeed, 0f, _vMax);
                _lookahead = Mathf.Max(0.2f, lookahead);
                _sampleStep = Mathf.Max(0.01f, sampleStep);
                _speedCount = Mathf.Max(2, speedCount);
                _maxSpan = Mathf.Max(1, maxSpan);
                _maxIters = Mathf.Max(1, maxIters);
                _planInterval = Mathf.Max(0f, planInterval);
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

                float pathFacing = _model.CurrentPart.Evaluate(_model.CurrentPartArc).Angle;
                float desired = _planMode == FacingMode.Target ? Bearing(current.Position, pathFacing) : pathFacing;
                float angle = Mathf.MoveTowardsAngle(current.Angle, desired, _wMax * dt);

                float target = PathGearUtility.GearOf(_model.CurrentPart) * _planSpeed;
                float speed = Mathf.MoveTowards(current.Speed, target, _aMax * dt);

                return new ControlInput(speed, angle);
            }

            private float Bearing(Vector2 position, float fallback)
            {
                Vector2 d = _target.Position - position;
                return d.sqrMagnitude < 1e-6f ? fallback : Mathf.Atan2(-d.x, d.y) * Mathf.Rad2Deg;
            }

            /// <summary>The expensive part: builds the window and the facing-mode map and searches the
            /// plan. Writes the first step's mode and speed. Throttled.</summary>
            private void Replan(DynamicState current)
            {
                List<WorldConfig> poses = _model.PeekAheadArc(_sampleStep, _lookahead);

                _planMode = FacingMode.Path;
                _planSpeed = 0f;

                if (poses.Count < 2)
                {
                    return;
                }

                MotionWindow window = new MotionWindow(poses, _grid, _target.Position);
                FacingModeMap map = new FacingModeMap(window, _grid, current.Angle, _speedCount,
                                                      _vMax, _aimSpeed, _wMax, _aMax, _maxSpan);

                FacingMode startMode = Mathf.Abs(Mathf.DeltaAngle(current.Angle, window.Aim[0])) <= map.AngleStep * 0.5f
                    ? FacingMode.Target
                    : FacingMode.Path;
                int start = map.Id(0, startMode, map.SpeedToIndex(current.Speed));

                IGoal goal = new GoalBase(new WindowEndAchivement(window.Count, map), new ZeroHeuristic());
                DirectTransition[] chain = _finder.FindPath(map, start, goal, _maxIters);

                if (chain != null && chain.Length > 0)
                {
                    map.Decode(chain[0].Neightbor, out _, out FacingMode mode, out int speed);
                    _planMode = mode;
                    _planSpeed = map.SpeedOf(speed);
                }
                else
                {
                    _planSpeed = _aimSpeed;
                }
            }
        }
    }
}
