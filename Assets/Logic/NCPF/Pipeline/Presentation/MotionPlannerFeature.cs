using NCPF.Pipeline.Application;
using NCPF.Domain;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// The MAIN pass. Every other pass is optional decoration — without this one the agent does not
    /// move at all, because the planner's path is pure geometry and carries neither speed nor
    /// facing. Both are born here, and TOGETHER: the turn affordable on a segment depends on how
    /// slowly it is crossed, so planning them apart is not possible.
    /// </summary>
    [Serializable]
    public class MotionPlannerFeature : MainPlannerFeature
    {
        [Header("Planning Window")]
        [SerializeField] private float _lookahead = 3f;
        [SerializeField] private float _sampleStep = 0.15f;
        [SerializeField] private int _speedCount = 5;
        [SerializeField] private int _maxSpan = 3;
        [SerializeField] private int _maxIterations = 4000;
        [SerializeField] private float _planInterval = 0.1f;

        [Header("Rules")]
        [SerializeField] private float _slowSpeedFactor = 0.25f;
        [SerializeField] private float _fastSpeedFactor = 1f;
        [SerializeField] private float _angleWeight = 0.02f;
        [SerializeField] private float _speedWeight = 0.5f;
        [SerializeField] private float _travelBias = 1.5f;

        public override MainSAPass CreatePass(PathAgentContext context)
        {
            DynamicAgent a = context.DynamicAgent;
            return new MotionPlannerPass(
                context.Model, context.Grid, context.Target,
                a.MaxAngledVelocity, a.MaxLinearAcceleration, a.MaxVelocity,
                a.MaxVelocity * Mathf.Clamp01(_slowSpeedFactor),
                a.MaxVelocity * Mathf.Clamp01(_fastSpeedFactor),
                _lookahead, _sampleStep, _speedCount, _maxSpan, _maxIterations,
                _angleWeight, _speedWeight, _travelBias, _planInterval);
        }

        /// <summary>
        /// The main pass body: searches (facing, speed) over the look-ahead window via an AV lattice
        /// under motion-rules weights, then chases the planned target every tick within the real
        /// dynamic limits.
        /// </summary>
        public class MotionPlannerPass : MainSAPass
        {
            private readonly PathAgentModel _model;
            private readonly IGridMap3D _grid;
            private readonly ITarget _target;
            private readonly float _wMax, _aMax, _vMax, _slow, _fast, _lookahead, _sampleStep;
            private readonly int _speedCount, _maxSpan, _maxIters;
            private readonly float _angleWeight, _speedWeight, _travelBias;
            private readonly float _planInterval;
            private readonly GoalPathFinderBase<DirectTransition> _finder = new GoalPathFinderBase<DirectTransition>();

            private float _lastPlanTime = float.NegativeInfinity;
            private float _planAngle, _planSpeed;
            private bool _hasPlan;

            public MotionPlannerPass(PathAgentModel model, IGridMap3D grid, ITarget target,
                                     float wMax, float aMax, float vMax, float slow, float fast,
                                     float lookahead, float sampleStep, int speedCount, int maxSpan, int maxIters,
                                     float angleWeight, float speedWeight, float travelBias, float planInterval)
            {
                _model = model;
                _grid = grid;
                _target = target;
                _wMax = Mathf.Max(1f, wMax);
                _aMax = Mathf.Max(0.001f, aMax);
                _vMax = Mathf.Max(0.001f, vMax);
                _slow = slow;
                _fast = fast;
                _lookahead = Mathf.Max(0.2f, lookahead);
                _sampleStep = Mathf.Max(0.01f, sampleStep);
                _speedCount = Mathf.Max(2, speedCount);
                _maxSpan = Mathf.Max(1, maxSpan);
                _maxIters = Mathf.Max(1, maxIters);
                _angleWeight = angleWeight;
                _speedWeight = speedWeight;
                _travelBias = travelBias;
                _planInterval = Mathf.Max(0f, planInterval);
            }

            public override ControlInput PlanControl(float dt, DynamicState current)
            {
                if (Time.time - _lastPlanTime >= _planInterval || !_hasPlan)
                {
                    _lastPlanTime = Time.time;
                    Replan(current);
                    _hasPlan = true;
                }

                float angle = Mathf.MoveTowardsAngle(current.Angle, _planAngle, _wMax * dt);
                float speed = Mathf.MoveTowards(current.Speed, _planSpeed, _aMax * dt);

                return new ControlInput(speed, angle);
            }

            /// <summary>The expensive part: builds the window, the AV-map and searches the plan.
            /// Writes the result into <see cref="_planAngle"/>/<see cref="_planSpeed"/>. Throttled.</summary>
            private void Replan(DynamicState current)
            {
                List<WorldConfig> poses = _model.PeekAheadArc(_sampleStep, _lookahead);

                _planAngle = current.Angle;
                _planSpeed = 0f;

                if (poses.Count < 2)
                {
                    return;
                }

                MotionWindow window = new MotionWindow(poses, _grid, _target.Position);
                SpacedMapAV map = new SpacedMapAV(window, _grid, _speedCount, _vMax, _wMax, _aMax, _maxSpan);

                MotionRulesGraph ruled = new MotionRulesGraph(
                    map,
                    new MotionRules(window, map, _slow, _fast,
                                    _angleWeight, _speedWeight, _travelBias));

                IGoal goal = new GoalBase(
                    new WindowEndAchivement(window.Count, map),
                    new WindowTimeHeuristic(window, map, _vMax));

                ConfigAV start = map.WorldToCellAV(new WorldConfigAV(0, current.Angle, current.Speed));
                start.Angle = map.NearestFreeAngle(0, start.Angle);

                DirectTransition[] chain = _finder.FindPath(ruled, map.CellAVToId(start), goal, _maxIters);

                if (chain != null && chain.Length > 0)
                {
                    WorldConfigAV plan = map.CellAVToWorld(map.IdToCellAV(chain[0].Neightbor));
                    _planAngle = plan.Angle;
                    _planSpeed = plan.Velocity;
                }
                else
                {
                    _planAngle = window.Travel[0];
                    _planSpeed = _slow;
                }

                int gear = PathGearUtility.GearOf(_model.CurrentPart);
                if (_planSpeed * gear < 0f)
                {
                    _planSpeed = -_planSpeed;
                }
            }
        }
    }
}
