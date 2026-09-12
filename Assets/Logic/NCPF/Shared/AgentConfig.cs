using NCPF.Domain;
using UnityEngine;

namespace NCPF.Shared.Presentation
{
    /// <summary>
    /// Asset port of the agent footprint: yields the collision <see cref="Agent"/>
    /// the bake convolves the world with, and exposes the footprint geometry both
    /// verticals draw with.
    /// </summary>
    public abstract class AgentConfig : ScriptableObject
    {
        public abstract Agent GetAgent(float cellSize);

        public abstract Vector2 FootprintSize { get; }

        public abstract Vector2 FootprintAnchor { get; }
    }
}
