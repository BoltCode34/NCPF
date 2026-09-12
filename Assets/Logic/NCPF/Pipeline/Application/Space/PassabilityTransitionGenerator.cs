using NCPF.Domain;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// Emits a node's transitions in ONE pass off pre-baked tables: collision comes from a
    /// <see cref="PrimitivePassability"/> mask, price and geometry-invariants from flat arrays
    /// addressed by the set's primitive id. Nothing is materialised for a primitive that fails the
    /// mask, and nothing carries a cell sweep — the sweep is what the mask was baked from.
    /// Replaces the projector-then-validator-then-pricer chain, where every primitive of the layer
    /// was built in full (offset cell array, wrapper, shape) and most were discarded by the next
    /// decorator, and where the price was found by hashing the library path reference.
    /// </summary>
    public class PassabilityTransitionGenerator : ITransitionGenerator
    {
        private const float EPS = 1e-3f;
        private const float KAPPA_EPS = 1e-6f;

        private readonly DiscretizedControlSet3D _controlSet;
        private readonly PrimitivePassability _passability;

        private readonly float[] _weight;
        private readonly float[] _length;
        private readonly Config[] _endCell;

        /// <summary>
        /// The tables are filled eagerly and never written again: price depends only on the
        /// primitive's own length and curvature peak (plus the fixed agent and knobs), never on the
        /// node it is projected onto — which is exactly why it can be precomputed at all.
        /// </summary>
        public PassabilityTransitionGenerator(DiscretizedControlSet3D controlSet,
                                              PrimitivePassability passability,
                                              DynamicAgent agent,
                                              float curvatureMultiplier = 0f,
                                              float baseMultiplier = 1f)
        {
            _controlSet = controlSet;
            _passability = passability;

            int ids = controlSet.IdCount;
            _weight = new float[ids];
            _length = new float[ids];
            _endCell = new Config[ids];

            float curvature = Mathf.Max(0f, curvatureMultiplier);
            float baseShare = Mathf.Max(0f, baseMultiplier);

            for (int layer = 0; layer < controlSet.AngleCount; layer++)
            {
                DiscretizedPath3D[] paths = controlSet.GetLayerSet(layer);
                if (paths == null)
                {
                    continue;
                }

                for (int index = 0; index < paths.Length; index++)
                {
                    DiscretizedPath3D primitive = paths[index];
                    int id = controlSet.IdOf(layer, index);

                    float length = primitive.Length;
                    float kappa = PrimitiveAnalyzer.Analyze(primitive.Path).KappaPeak;

                    _length[id] = length;
                    _weight[id] = TravelWeight(length, kappa, agent, curvature, baseShare);
                    _endCell[id] = primitive.IdSequence[primitive.IdSequence.Length - 1];
                }
            }
        }

        public TransitionData[] GetPrimitives(Config startCell, IGridMap3D grid)
        {
            DiscretizedPath3D[] paths = _controlSet.GetLayerSet(startCell.Angle);
            if (paths == null)
            {
                return System.Array.Empty<TransitionData>();
            }

            int nodeId = grid.Cell3DToId(startCell);

            List<TransitionData> results = null;
            for (int index = 0; index < paths.Length; index++)
            {
                if (!_passability.Passable(nodeId, index))
                {
                    continue;
                }

                int id = _controlSet.IdOf(startCell.Angle, index);
                if (_length[id] <= EPS)
                {
                    continue;
                }

                Config end = _endCell[id];
                int endId = grid.CellToId(end.X + startCell.X, end.Y + startCell.Y, end.Angle);

                results ??= new List<TransitionData>(paths.Length);
                results.Add(new TransitionData(nodeId, endId, id, _weight[id]));
            }

            return results == null
                ? System.Array.Empty<TransitionData>()
                : results.ToArray();
        }

        /// <summary>
        /// Two-term price, unchanged from <see cref="CurvateEdgeGenerator"/>: honest time
        /// baseMultiplier·L/cap, where cap cuts the speed ceiling by the same three limits as the PV
        /// ladder (v ≤ vMax, v ≤ √(a/κ), v ≤ w_max/κ), plus the mission tightness tax κ²·L.
        /// </summary>
        private static float TravelWeight(float length, float kappa, DynamicAgent agent,
                                          float curvatureMultiplier, float baseMultiplier)
        {
            float vMax = Mathf.Max(EPS, agent.MaxVelocity);

            float cap = vMax;
            if (kappa > KAPPA_EPS)
            {
                float aMax = Mathf.Max(0.001f, agent.MaxLinearAcceleration);
                float wMaxRad = Mathf.Max(1f, agent.MaxAngledVelocity) * Mathf.Deg2Rad;
                cap = Mathf.Min(vMax, Mathf.Min(Mathf.Sqrt(aMax / kappa), wMaxRad / kappa));
            }

            float weight = baseMultiplier * length / cap;

            if (curvatureMultiplier > 0f && kappa > KAPPA_EPS)
            {
                weight += curvatureMultiplier * kappa * kappa * length;
            }

            return weight;
        }
    }
}
