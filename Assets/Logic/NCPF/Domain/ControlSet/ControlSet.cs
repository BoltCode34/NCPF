using System.Collections.Generic;
using System.Linq;

namespace NCPF.Domain
{
    /// <summary>
    /// The geometric primitive library: layers of paths keyed by the START
    /// direction index only — speed is a dimension of the search graph, not
    /// of the shape library.
    /// </summary>
    public class ControlSet<T> where T : IPath
    {
        protected Dictionary<int, T[]> _pathSet;

        public int Count => _pathSet?.Count ?? 0;
        public int MaxAngle { get; private set; }
        public int AngleCount => MaxAngle + 1;

        /// <summary>
        /// Primitives per layer the id encoding reserves — the widest layer loaded. Ids stay dense,
        /// so a table addressed by id is a flat array, and it is the same value a passability mask
        /// reserves per node.
        /// </summary>
        public int Stride { get; private set; }

        /// <summary>Total id space: <see cref="AngleCount"/> × <see cref="Stride"/>.</summary>
        public int IdCount => AngleCount * Stride;

        public int PathCount(int angle) => _pathSet[angle].Length;

        public void LoadLayer(int angle, params T[] paths)
        {
            _pathSet ??= new Dictionary<int, T[]>();
            _pathSet[angle] = paths;
            if (MaxAngle < angle)
            {
                MaxAngle = angle;
            }
            if (paths != null && paths.Length > Stride)
            {
                Stride = paths.Length;
            }
        }

        /// <summary>
        /// The set is the only authority on what an id means: callers pass ids around and come back
        /// here to resolve them. The encoding stays an implementation detail on purpose — nothing
        /// outside may take an id apart.
        /// </summary>
        public int IdOf(int layer, int index) => layer * Stride + index;

        public int LayerOf(int id) => Stride <= 0 ? 0 : id / Stride;

        public int IndexOf(int id) => Stride <= 0 ? 0 : id % Stride;

        public T GetById(int id)
        {
            T[] paths = GetLayerSet(LayerOf(id));
            if (paths == null)
            {
                return default;
            }
            int index = IndexOf(id);
            return index >= 0 && index < paths.Length ? paths[index] : default;
        }

        public virtual T[] GetLayerSet(int angle)
        {
            if (_pathSet == null)
            {
                return null;
            }
            return _pathSet.TryGetValue(angle, out T[] paths) ? paths : null;
        }

        public virtual int[] GetLayers()
        {
            if (_pathSet == null)
            {
                return System.Array.Empty<int>();
            }
            return _pathSet.Keys.OrderBy(a => a).ToArray();
        }
    }
}
