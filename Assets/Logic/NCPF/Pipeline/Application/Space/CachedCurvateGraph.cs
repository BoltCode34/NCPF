using NCPF.Domain;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// Memoises a node's outgoing transitions and its free flag. Deriving them depends only on the
    /// node and the (static) map, never on search state — yet A* re-derives them on every expansion,
    /// in every eps rung and on every replan over what is largely the same neighbourhood. The cache
    /// lives as long as the graph instance, so composing a fresh graph after a map rebuild drops it
    /// with no invalidation wiring.
    /// Concurrent because searches run on the thread pool and can overlap.
    /// MUST SIT OUTERMOST: decorators below price edges by mutating the array in place
    /// (<see cref="RearGearGraph"/>), and a cache beneath one would let the penalty compound on
    /// every hit. Callers must treat the returned array as READ-ONLY — it is shared.
    /// </summary>
    public class CachedCurvateGraph : ICurvateGraph
    {
        private readonly ICurvateGraph _decoratee;
        private readonly int _edgeBudget;
        private readonly ConcurrentDictionary<int, TransitionData[]> _edges = new();
        private readonly ConcurrentDictionary<int, bool> _free = new();
        private int _cachedEdges;

        /// <summary>
        /// Budgeted in EDGES, not nodes: outdegree runs from a handful in a corridor to the whole
        /// layer in the open, so a node count would bound nothing. An edge is 16 bytes, so the
        /// default is about 32 MB of plan cache.
        /// </summary>
        public CachedCurvateGraph(ICurvateGraph decoratee, int edgeBudget = 2000000)
        {
            _decoratee = decoratee;
            _edgeBudget = Mathf.Max(1, edgeBudget);
        }

        public TransitionData[] GetNeightbors(int id)
        {
            if (_edges.TryGetValue(id, out TransitionData[] cached))
            {
                return cached;
            }

            TransitionData[] edges = _decoratee.GetNeightbors(id);
            if (_edges.TryAdd(id, edges)
                && Interlocked.Add(ref _cachedEdges, edges.Length) > _edgeBudget)
            {
                _edges.Clear();
                Interlocked.Exchange(ref _cachedEdges, 0);
            }
            return edges;
        }

        public bool PointFree(int id)
        {
            if (_free.TryGetValue(id, out bool cached))
            {
                return cached;
            }

            bool free = _decoratee.PointFree(id);
            _free.TryAdd(id, free);
            return free;
        }
    }
}
