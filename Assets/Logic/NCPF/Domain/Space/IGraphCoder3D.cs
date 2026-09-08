using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// Encodes and decodes between a graph node id and a 3D world pose
    /// (x, y, heading).
    /// </summary>
    public interface IGraphCoder3D : IGraph
    {
        public WorldConfig IDToWorld(int id);
        public int WorldToID(Vector2 pos, float angle);
        public int WorldToID(WorldConfig config);
    }
}
