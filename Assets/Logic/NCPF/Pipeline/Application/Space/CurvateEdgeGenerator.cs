using NCPF.Domain;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// The EDGE GENERATOR: the single owner of a transition's PRICE. Runs the primitive set
    /// through the projector chain (pure projection + validator) and prices the survivors in one
    /// formula: honest time + the mission tightness tax. A survivor is recognized by its library
    /// REFERENCE (<see cref="PathOffset.Inner"/>) — analysis is taken once over the library
    /// (PrimitiveAnalyzer, eagerly in the constructor), so the chain may filter freely. There is
    /// NO in-place turn: the heading changes only by driving a curve. Analysis is immutable after
    /// the constructor — thread safety by contract, not by locks.
    /// </summary>
    /// <remarks>
    /// SUPERSEDED by <see cref="PassabilityTransitionGenerator"/> and no longer an
    /// <see cref="ITransitionGenerator"/>: that contract now yields <see cref="TransitionData"/>,
    /// which names a primitive by id instead of carrying its shape. Kept as the reference
    /// implementation of the projector-chain pricing.
    /// </remarks>
    public class CurvateEdgeGenerator
    {
        private const float EPS = 1e-3f;
        private const float KAPPA_EPS = 1e-6f;

        private readonly IPrimitivesProjector3D _projector;
        private readonly int _layersCount;
        private readonly Dictionary<IPath, float> _kappa;
        private readonly DynamicAgent _agent;
        private readonly float _curvatureMultiplier;
        private readonly float _baseMultiplier;

        /// <summary>
        /// The generator's effect-strength knobs. baseMultiplier is the share of honest time L/cap
        /// (1 = honest, 0 takes metres out of the price; below 1 breaks the positional heuristic's
        /// admissibility). curvatureMultiplier is the MISSION tightness tax κ_peak²·L (s·m: 0 =
        /// neutral, ~0.1 makes a tight arc lose to a gentle one at equal turn). The gear knob lives
        /// in <see cref="RearGearGraph"/>, not here.
        /// </summary>
        public CurvateEdgeGenerator(IPrimitivesProjector3D projector,
                                    DiscretizedControlSet3D controlSet,
                                    DynamicAgent agent,
                                    float curvatureMultiplier = 0f,
                                    float baseMultiplier = 1f)
        {
            _projector = projector;
            _agent = agent;
            _curvatureMultiplier = Mathf.Max(0f, curvatureMultiplier);
            _baseMultiplier = Mathf.Max(0f, baseMultiplier);

            _layersCount = controlSet.AngleCount;
            _kappa = new Dictionary<IPath, float>();
            for (int layer = 0; layer < _layersCount; layer++)
            {
                DiscretizedPath3D[] entries = controlSet.GetLayerSet(layer);
                if (entries == null) continue;
                for (int i = 0; i < entries.Length; i++)
                {
                    _kappa[entries[i].Path] = PrimitiveAnalyzer.Analyze(entries[i].Path).KappaPeak;
                }
            }
        }

        public CurvateTransition[] GetPrimitives(Config startCell, IGridMap3D grid)
        {
            List<CurvateTransition> results = new();

            DiscretizedPath3D[] survivors = _projector.GetPrimitives(startCell, grid);

            for (int i = 0; i < survivors.Length; i++)
            {
                DiscretizedPath3D primitive = survivors[i];

                if (primitive.Length <= EPS)
                {
                    continue;
                }

                IPath source = primitive.Path is PathOffset offset ? offset.Inner : primitive.Path;
                _kappa.TryGetValue(source, out float kappa);

                float weight = TravelWeight(primitive.Length, kappa);
                Config end = primitive.IdSequence[primitive.IdSequence.Length - 1];
                results.Add(new CurvateTransition(grid.Cell3DToId(end), primitive, weight));
            }
            return results.ToArray();
        }

        /// <summary>
        /// Two-term price: baseMultiplier·L/cap + curvatureMultiplier·κ_peak²·L. The base is honest
        /// time — the cap cuts the speed ceiling at search time with the same three limits as the PV
        /// ladder (v ≤ vMax, v ≤ √(a/κ_peak), v ≤ w_max/κ_peak). The second term is the MISSION
        /// tightness tax: at an equal turn angle a tight arc pays in proportion to κ; straights pay
        /// nothing.
        /// </summary>
        private float TravelWeight(float length, float kappa)
        {
            float vMax = Mathf.Max(EPS, _agent.MaxVelocity);

            float cap = vMax;
            if (kappa > KAPPA_EPS)
            {
                float aMax = Mathf.Max(0.001f, _agent.MaxLinearAcceleration);
                float wMaxRad = Mathf.Max(1f, _agent.MaxAngledVelocity) * Mathf.Deg2Rad;
                cap = Mathf.Min(vMax, Mathf.Min(Mathf.Sqrt(aMax / kappa), wMaxRad / kappa));
            }

            float weight = _baseMultiplier * length / cap;

            if (_curvatureMultiplier > 0f && kappa > KAPPA_EPS)
            {
                weight += _curvatureMultiplier * kappa * kappa * length;
            }

            return weight;
        }
    }
}
