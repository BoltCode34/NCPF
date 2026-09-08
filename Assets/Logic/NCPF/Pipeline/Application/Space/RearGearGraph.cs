using NCPF.Domain;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// A graph decorator — the MISSION-tier gear preference: rear-first entries get their weight
    /// multiplied by gearPenalty (1 = neutral). To the model reversing is exactly as legal and as
    /// fast as going forward; to the mission it is exceptional (eyes face forward). The multiplier
    /// makes CASUAL reversing expensive while the exceptional case still wins. Rear-first is
    /// recognized by the IRearFirstPath marker beneath the wrappers (PathGearUtility.IsRearFirst).
    /// </summary>
    public class RearGearGraph : ICurvateGraph
    {
        private readonly ICurvateGraph _decoratee;
        private readonly float _gearPenalty;

        public RearGearGraph(ICurvateGraph decoratee, float gearPenalty)
        {
            _decoratee = decoratee;
            _gearPenalty = Mathf.Max(1f, gearPenalty);
        }

        public bool PointFree(int id) => _decoratee.PointFree(id);

        public CurvateTransition[] GetNeightbors(int id)
        {
            CurvateTransition[] edges = _decoratee.GetNeightbors(id);
            for (int i = 0; i < edges.Length; i++)
            {
                if (PathGearUtility.IsRearFirst(edges[i].Path))
                {
                    edges[i].Weight *= _gearPenalty;
                }
            }
            return edges;
        }
    }
}
