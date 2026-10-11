using NCPF.Domain;
using NCPF.Pipeline.Application;
using NCPF.Shared.Presentation;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// Builds the search's data side from the inspector ports: map → discretized control set →
    /// CurvateMap3D → RearGearGraph → ε-ladder finders, packed into a <see cref="PathAgentContext"/>.
    /// Also tracks a signature of the knobs the planner was wired from, so the shell can detect an
    /// inspector change and rebuild. Dependency injection lives here — the shell calls Build when no
    /// resolver wired it, and CreateSimContext for the edit-time rollout.
    /// </summary>
    public class NcpfGraphComposer
    {
        private PlannerConfig _signature;
        private bool _hasSignature;

        /// <summary>Builds a fresh context from the current inspector config. Does not call back
        /// into the shell — the caller wires the brain from the returned context.</summary>
        public PathAgentContext Build(PlannerConfig config, ITarget target)
        {
            IGridMap3D map3D = new SpacedMap3D(config.Grid, config.Map.Read());
            ControlSet<IPath> shapes = config.ControlSet.Read(map3D);
            DiscretizedControlSet3D discretized =
                new DiscretizedControlSet3DConverter().Create(map3D, shapes);

            PrimitivePassability passability = new PassabilityBaker().Bake(map3D, discretized);

            ICurvateGraph graph = new CachedCurvateGraph(
                new RearGearGraph(
                    new CurvateMap3D(
                        map3D,
                        new PassabilityTransitionGenerator(
                            discretized,
                            passability,
                            config.DynamicAgent.Agent,
                            config.CurvaturePenalty,
                            config.BaseTimePenalty)),
                    discretized,
                    config.RearGearPenalty));

            PlanGeometry geometry = new PlanGeometry(discretized, map3D);

            IAsyncGoalPathFinder<TransitionData> asyncPathFinder =
                new EpsilonLadderFinder(new CurvateGoalPathFinder(), config.EpsilonLadder, config.AttemptBudget);
            IGoalPathFinder<TransitionData> pathFinder = new CurvateGoalPathFinder();

            PathAgentModel model = new PathAgentModel();
            PathAgentContext context = new PathAgentContext(
                model, config.DynamicAgent.Agent, asyncPathFinder, pathFinder, map3D, graph, target, geometry);

            _signature = config;
            _hasSignature = true;
            return context;
        }

        /// <summary>Records the current config as the wired signature (for a context the shell did
        /// not build here, e.g. one injected by the composition root).</summary>
        public void CaptureSignature(PlannerConfig config)
        {
            _signature = config;
            _hasSignature = true;
        }

        /// <summary>True if the signature was captured and the current config differs from it.</summary>
        public bool SignatureChanged(PlannerConfig config)
            => _hasSignature && !config.Same(_signature);

        /// <summary>A throwaway context for edit-time rollout: a fresh model over the live map,
        /// graph, finders, target and agent. The simulation follower runs on it without touching
        /// the live model.</summary>
        public PathAgentContext CreateSimContext(PathAgentContext live)
            => new PathAgentContext(
                new PathAgentModel(),
                live.DynamicAgent,
                live.AsyncPathFinder,
                live.PathFinder,
                live.Grid,
                live.Graph,
                live.Target,
                live.Geometry);

        /// <summary>The inspector-side knobs and ports the planner was wired from this frame.</summary>
        public struct PlannerConfig
        {
            public Unity2DGrid Grid;
            public MapAsset Map;
            public ControlSetAsset ControlSet;
            public DynamicAgentConfig DynamicAgent;
            public float RearGearPenalty;
            public float CurvaturePenalty;
            public float BaseTimePenalty;
            public int AttemptBudget;
            public float[] EpsilonLadder;

            public bool Same(PlannerConfig other)
            {
                if (!ReferenceEquals(Grid, other.Grid)
                    || !ReferenceEquals(Map, other.Map)
                    || !ReferenceEquals(ControlSet, other.ControlSet)
                    || !ReferenceEquals(DynamicAgent, other.DynamicAgent))
                {
                    return false;
                }
                if (RearGearPenalty != other.RearGearPenalty
                    || CurvaturePenalty != other.CurvaturePenalty
                    || BaseTimePenalty != other.BaseTimePenalty
                    || AttemptBudget != other.AttemptBudget)
                {
                    return false;
                }
                if (EpsilonLadder == null || other.EpsilonLadder == null
                    || EpsilonLadder.Length != other.EpsilonLadder.Length)
                {
                    return false;
                }
                for (int i = 0; i < EpsilonLadder.Length; i++)
                {
                    if (EpsilonLadder[i] != other.EpsilonLadder[i]) return false;
                }
                return true;
            }
        }
    }
}
