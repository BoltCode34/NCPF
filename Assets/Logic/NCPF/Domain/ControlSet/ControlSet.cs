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

        public int PathCount(int angle) => _pathSet[angle].Length;

        public void LoadLayer(int angle, params T[] paths)
        {
            _pathSet ??= new Dictionary<int, T[]>();
            _pathSet[angle] = paths;
            if (MaxAngle < angle)
            {
                MaxAngle = angle;
            }
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
