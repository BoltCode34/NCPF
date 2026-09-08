using NCPF.Domain;
using UnityEngine;

namespace NCPF.Shared
{
    /// <summary>
    /// Storage port for a baked control set. Fields typed by this base accept any
    /// concrete asset Unity binds to them; read and write are pure geometry.
    /// </summary>
    public abstract class ControlSetAsset : ScriptableObject
    {
        public abstract ControlSet<IPath> Read(IGridGeometry3D space);

        public abstract IPath[] ReadLayer(int angle, IGridGeometry3D space);

        public abstract void Write(ControlSet<IPath> controlSet, IGridGeometry3D space);
    }
}
