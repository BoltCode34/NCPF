using NCPF.Pipeline.Application;
using NCPF.Domain;
using NCPF.Shared;
using System.Collections.Generic;
using UnityEngine;
using Core.Shared.Extentions;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// The planning shell: a MonoBehaviour that wires the search brain, the behaviour bus and the
    /// feature container, drives the body from the model in FixedUpdate, and draws the plan. All
    /// planning logic lives in <see cref="PathAgentPipeline"/> (app); map composition in
    /// <see cref="NcpfGraphComposer"/>; feature wiring in <see cref="PathFeatureContainer"/>.
    /// ExecuteAlways keeps it working outside play mode for editor experiments.
    /// </summary>
    [ExecuteAlways]
    [DefaultExecutionOrder(-10)]
    public class PathPlanner : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Unity2DGrid _grid;
        [SerializeField] private MapAsset _mapContainer;
        [SerializeField] private ControlSetAsset _controlSetContainer;
        [SerializeField] private DynamicAgentConfig _dynamicAgent;
        [SerializeField] private TransformTarget _target;
        [SerializeField] private AgentConfig _agentShape;

        [Header("Search")]
        [SerializeField] private float[] _epsilonLadder = { 3f, 2f, 4f, 1.5f };
        [SerializeField] private int _attemptBudget = 400;
        [SerializeField] private float _replanInterval = 0.5f;

        [Header("NCPF Preferences")]
        [SerializeField, Min(1f)] private float _rearGearPenalty = 1f;
        [SerializeField, Min(0f)] private float _curvaturePenalty = 0f;
        [SerializeField, Min(0f)] private float _baseTimePenalty = 1f;

        [Header("Features")]
        [SerializeReference] [SubclassSelector] private AchivementFeature _achivementFeature;
        [SerializeReference] [SubclassSelector] private HeuristicFeature _heuristicFeature;
        [SerializeReference] [SubclassSelector] private MainPlannerFeature _mainPlannerFeature;
        [SerializeReference] [SubclassSelector] private List<PlannerOverrideFeature> _overrides = new List<PlannerOverrideFeature>();

        [Header("Runtime")]
        [SerializeField] private float _timeScale = 1;

        [Header("Drawing")]
        [SerializeField] private bool _draw;
        [SerializeField] private bool _drawPoints;
        [SerializeField] private int _stepCount = 100;
        [SerializeField] private bool _simulate;
        [SerializeField] private float _simDt = 0.05f;
        [SerializeField] private int _simMaxSteps = 2000;
        [SerializeField] private int _bodyEvery = 8;

        [Header("Model State")]
        [SerializeField] private bool _dbgHasPath;
        [SerializeField] private bool _dbgCompleted;
        [SerializeField] private Vector2 _dbgPosition;
        [SerializeField] private float _dbgSpeed;
        [SerializeField] private float _dbgNose;
        [SerializeField] private float _dbgTangent;
        [SerializeField] private int _dbgGear;
        [SerializeField] private int _dbgNextGear;
        [SerializeField] private int _dbgPartIndex;
        [SerializeField] private int _dbgPartsCount;
        [SerializeField] private float _dbgPartArc;
        [SerializeField] private float _dbgPartLeft;

        private NcpfGraphComposer _composer;
        private PlannerStandards _standards;
        private PathFeatureContainer _container;
        private BehaviorBus _bus;
        private PathAgentPipeline _brain;
        private PathAgentContext _context;
        private TargetRefHolder _targetRef;

        public ITarget Target
        {
            get => _targetRef?.Target;
            set { if (_targetRef != null) _targetRef.Target = value; }
        }
        public IGridMap3D Map => _context?.Grid;
        public ICurvateGraph Graph => _context?.Graph;
        public DynamicAgent Agent => _context?.DynamicAgent;
        public TransformTarget Prey { get => _target; set => _target = value; }
        public PathAgentContext Context => _context;

        public float RearGearPenalty => _rearGearPenalty;
        public float CurvaturePenalty => _curvaturePenalty;
        public float BaseTimePenalty => _baseTimePenalty;

        private void OnEnable()
        {
            if (_standards == null) _standards = new PlannerStandards();
            if (_composer == null) _composer = new NcpfGraphComposer();
        }

        private void Start()
        {
            if (_context == null)
            {
                InjectStandardDependency();
            }
        }

        /// <summary>Inspector-fallback bootstrap: builds the map/graph/finders and wires the brain
        /// when no composition root has injected a context. No-op once wired.</summary>
        [ContextMenu("Inject Standard Dependency")]
        public void InjectStandardDependency()
        {
            if (_context != null) return;
            if (_grid == null || _controlSetContainer == null || _mapContainer == null) return;

            _targetRef = new TargetRefHolder(_target);
            PathAgentContext ctx = _composer.Build(BuildConfig(), _targetRef);
            ConstructCore(ctx, _attemptBudget, null);
        }

        /// <summary>Full re-wire: breaks the in-flight search, drops the plan and rebuilds the map
        /// from the current inspector knobs. The body's state survives via a rebuild seed.</summary>
        [ContextMenu("Rebuild Map")]
        public void RebuildMap()
        {
            DynamicState? seed = _brain != null && _brain.Follower.HasPath
                ? _brain.Follower.CurrentDynamicState
                : _brain?.RebuildSeed;
            _brain?.Break();
            _context = null;
            _brain = null;
            _bus = null;

            if (_targetRef == null) _targetRef = new TargetRefHolder(_target);
            PathAgentContext ctx = _composer.Build(BuildConfig(), _targetRef);
            ConstructCore(ctx, _attemptBudget, seed);
        }

        /// <summary>Wires the brain from an externally built context (the composition root calls
        /// this with its own finders; the shell calls ConstructCore for the fallback path).</summary>
        public void Construct(PathAgentContext context, int attemptBudget)
        {
            ConstructCore(context, attemptBudget, null);
        }

        private void ConstructCore(PathAgentContext context, int attemptBudget, DynamicState? seed)
        {
            if (_composer == null) _composer = new NcpfGraphComposer();
            if (_standards == null) _standards = new PlannerStandards();

            _context = context;
            _targetRef = context.Target as TargetRefHolder ?? new TargetRefHolder(context.Target);
            _composer.CaptureSignature(BuildConfig());

            _bus = new BehaviorBus();
            PathFollowAgent follower = new PathFollowAgent(context.Model, _bus.Main, null);
            _bus.Attach(follower);

            BaseGoalBuilder goal = new BaseGoalBuilder
            {
                AchivementBuilder = _bus.Achivement,
                HeuristicBuilder = _bus.Heuristic
            };
            PathFindingAgent search = new PathFindingAgent(
                context.Grid, context.Graph, context.AsyncPathFinder, context.PathFinder,
                goal, context.DynamicAgent, attemptBudget);

            _brain = new PathAgentPipeline(search, follower, context.Grid, context.Target, _replanInterval, seed);
            _container = new PathFeatureContainer(_standards);
            _container.Bind(_bus, context, _mainPlannerFeature, _achivementFeature, _heuristicFeature, _overrides);
        }

        public void Update()
        {
            if (_brain == null)
            {
                InjectStandardDependency();
            }
            else if (_composer.SignatureChanged(BuildConfig()))
            {
                RebuildMap();
            }

            if (_context != null && _bus != null)
            {
                _container.Bind(_bus, _context, _mainPlannerFeature, _achivementFeature, _heuristicFeature, _overrides);
            }
            if (_brain != null)
            {
                _brain.Update(BodyState());
            }
        }

        public void FixedUpdate()
        {
            if (!UnityEngine.Application.isPlaying) return;
            if (_context != null) LogModelState();
            if (_brain == null || _brain.Path == null) return;

            DynamicState state = _brain.Follower.GetNext(Time.fixedDeltaTime * _timeScale);
            transform.position = state.Position;
            transform.eulerAngles = new Vector3(0, 0, state.Angle);
        }

        public void OnDrawGizmos()
        {
            if (_context == null)
            {
                InjectStandardDependency();
                if (_context == null) return;
            }
            Gizmos.DrawSphere(transform.position, 0.1f);
            if (Target != null) Gizmos.DrawSphere(Target.Position, 0.1f);
            DrawPath();
        }

        public void DrawPath()
        {
            if (_brain == null || _brain.Path == null || _brain.Path.Length == 0 || !_draw || _context == null) return;

            if (_drawPoints)
            {
                Gizmos.color = Color.yellow;
                CurvateTransition[] path = _brain.Path;
                for (int i = 0; i < path.Length - 1; i++)
                {
                    Vector2 a = _context.Grid.Cell3DToWorld(_context.Grid.IdToCell3D(path[i].Neightbor)).Position;
                    Vector2 b = _context.Grid.Cell3DToWorld(_context.Grid.IdToCell3D(path[i + 1].Neightbor)).Position;
                    Gizmos.DrawLine(a, b);
                    Gizmos.DrawSphere(b, 0.05f);
                }
            }

            if (_simulate) DrawSimulated();
            else DrawGeometry();
        }

        private void DrawGeometry()
        {
            CurvateTransition[] path = _brain.Path;
            Gizmos.color = Color.green;
            int steps = Mathf.Max(1, _stepCount / path.Length);
            for (int i = 0; i < path.Length; i++)
            {
                DiscretizedPath3D shape = path[i].Path;
                float step = shape.Length / steps;
                WorldConfig last = shape.Evaluate(0f);
                for (int j = 1; j <= steps; j++)
                {
                    WorldConfig next = shape.Evaluate(j * step);
                    Gizmos.DrawLine(last.Position, next.Position);
                    last = next;
                }
            }
        }

        private void DrawSimulated()
        {
            PathAgentContext simContext = _composer.CreateSimContext(_context);
            MainPlannerFeature planner = _mainPlannerFeature ?? _standards.Main;
            PathFollowAgent sim = new PathFollowAgent(simContext.Model, planner.CreatePass(simContext), null);

            CurvateTransition[] path = _brain.Path;
            WorldConfig head = path[0].Path.Start;
            sim.StartFollowing(path, new DynamicState(head.Position, head.Angle, 0f, 0f));

            List<DynamicState> states = PlanRollout.Run(sim, Mathf.Max(0.001f, _simDt), _simMaxSteps);
            float vMax = Mathf.Max(0.001f, _dynamicAgent.Agent.MaxVelocity);
            int bodyEvery = Mathf.Max(1, _bodyEvery);

            DynamicState last = states[0];
            for (int i = 1; i < states.Count; i++)
            {
                DynamicState next = states[i];
                Gizmos.color = Color.Lerp(Color.red, Color.green, Mathf.Abs(next.Speed) / vMax);
                Gizmos.DrawLine(last.Position, next.Position);

                if (_agentShape != null && (i - 1) % bodyEvery == 0)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos2D.DrawWireQuad(next.Position, _agentShape.FootprintSize, next.Angle, _agentShape.FootprintAnchor);
                    Gizmos.DrawSphere(next.Position, 0.05f);
                }
                last = next;
            }
        }

        /// <summary>A snapshot of the agent model into inspector fields — stupor diagnostics without
        /// logs: signed speed, cursor (index/arc/left) and the gear of the current and next primitive.</summary>
        private void LogModelState()
        {
            PathAgentModel model = _context.Model;
            DynamicState state = model.CurrentDynamicState;
            _dbgHasPath = model.HasPath;
            _dbgCompleted = model.Completed;
            _dbgPosition = state.Position;
            _dbgSpeed = state.Speed;
            _dbgNose = state.Angle;
            _dbgTangent = state.VelocityAngle;

            _dbgPartIndex = model.CurrentPartIndex;
            _dbgPartsCount = model.Parts.Count;
            _dbgPartArc = model.CurrentPartArc;
            _dbgGear = 0;
            _dbgNextGear = 0;
            _dbgPartLeft = 0f;

            IPath part = model.CurrentPart;
            if (part != null)
            {
                _dbgGear = PathGearUtility.GearOf(part);
                _dbgPartLeft = part.Length - model.CurrentPartArc;
            }

            if (_dbgPartIndex + 1 < _dbgPartsCount)
            {
                _dbgNextGear = PathGearUtility.GearOf(model.Parts[_dbgPartIndex + 1]);
            }
        }

        public void SetAchivement(AchivementFeature feature)
        {
            _achivementFeature = feature ?? _standards.Achivement;
        }

        public void SetHeuristic(HeuristicFeature feature)
        {
            _heuristicFeature = feature ?? _standards.Heuristic;
        }

        public void SetMainPass(MainPlannerFeature feature)
        {
            _mainPlannerFeature = feature ?? _standards.Main;
        }

        public void AddSAOverride(PlannerOverrideFeature feature)
        {
            if (feature != null && !_overrides.Contains(feature)) _overrides.Add(feature);
        }

        public void RemoveSAOverride(PlannerOverrideFeature feature)
        {
            if (feature != null && _overrides.Contains(feature)) _overrides.Remove(feature);
        }

        private DynamicState BodyState()
            => new DynamicState((Vector2)transform.position, transform.eulerAngles.z, 0f, 0f);

        private NcpfGraphComposer.PlannerConfig BuildConfig() => new NcpfGraphComposer.PlannerConfig
        {
            Grid = _grid,
            Map = _mapContainer,
            ControlSet = _controlSetContainer,
            DynamicAgent = _dynamicAgent,
            RearGearPenalty = _rearGearPenalty,
            CurvaturePenalty = _curvaturePenalty,
            BaseTimePenalty = _baseTimePenalty,
            AttemptBudget = _attemptBudget,
            EpsilonLadder = _epsilonLadder
        };
    }
}
