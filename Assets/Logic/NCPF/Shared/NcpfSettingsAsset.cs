using UnityEngine;

namespace NCPF.Shared.Presentation
{
    /// <summary>
    /// Storage port for package-wide runtime settings, read by the planner bootstrap. Typed by this
    /// base so that no assembly references NCPF.Config: Unity binds the concrete asset.
    /// </summary>
    public abstract class NcpfSettingsAsset : ScriptableObject
    {
        /// <summary>Whether planners with matching inputs share their map, control set and graph.</summary>
        public abstract bool CombineDependencies { get; }
    }
}
