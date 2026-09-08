using NCPF.Domain;
using UnityEngine;

namespace NCPF.Shared
{
    /// <summary>
    /// Storage port for a baked C-space map. Fields typed by this base accept any
    /// concrete asset Unity binds to them.
    /// </summary>
    public abstract class MapAsset : ScriptableObject
    {
        public abstract Map3D Read();

        public abstract void Write(Map3D map);
    }
}
