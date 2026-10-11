using NCPF.Domain;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// A graph decorator — the MISSION-tier gear preference: rear-first entries get their weight
    /// multiplied by gearPenalty (1 = neutral). To the model reversing is exactly as legal and as
    /// fast as going forward; to the mission it is exceptional (eyes face forward). The multiplier
    /// makes CASUAL reversing expensive while the exceptional case still wins.
    /// Rear-first is resolved ONCE per library primitive into a flag addressed by id: the search
    /// edge carries no shape to inspect, and walking the wrapper chain per edge per expansion is
    /// exactly the kind of work that has no business in the hot loop.
    /// </summary>
    public class RearGearGraph : ICurvateGraph
    {
        private readonly ICurvateGraph _decoratee;
        private readonly float _gearPenalty;
        private readonly bool[] _rearFirst;

        public RearGearGraph(ICurvateGraph decoratee, DiscretizedControlSet3D controlSet, float gearPenalty)
        {
            _decoratee = decoratee;
            _gearPenalty = Mathf.Max(1f, gearPenalty);

            _rearFirst = new bool[controlSet.IdCount];
            for (int layer = 0; layer < controlSet.AngleCount; layer++)
            {
                DiscretizedPath3D[] paths = controlSet.GetLayerSet(layer);
                if (paths == null)
                {
                    continue;
                }

                for (int index = 0; index < paths.Length; index++)
                {
                    _rearFirst[controlSet.IdOf(layer, index)] = PathGearUtility.IsRearFirst(paths[index].Path);
                }
            }
        }

        public bool PointFree(int id) => _decoratee.PointFree(id);

        public TransitionData[] GetNeightbors(int id)
        {
            TransitionData[] edges = _decoratee.GetNeightbors(id);
            for (int i = 0; i < edges.Length; i++)
            {
                int primitive = edges[i].PrimitiveId;
                if (primitive >= 0 && primitive < _rearFirst.Length && _rearFirst[primitive])
                {
                    edges[i].Weight *= _gearPenalty;
                }
            }
            return edges;
        }
    }
}
