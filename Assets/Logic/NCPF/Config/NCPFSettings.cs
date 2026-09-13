using NCPF.Shared.Presentation;
using UnityEngine;
using UnityEngine.Serialization;

namespace NCPF.Config
{
    /// <summary>
    /// Package-wide runtime settings. Loaded from a Resources folder under the name "NCPFSettings";
    /// when no such asset exists, the bootstrap falls back to the defaults below.
    /// </summary>
    [CreateAssetMenu(fileName = "NCPFSettings", menuName = "NCPF/Settings")]
    public class NCPFSettings : NcpfSettingsAsset
    {
        [FormerlySerializedAs("CombineDependencies")]
        [SerializeField] private bool _combineDependencies = true;

        public override bool CombineDependencies => _combineDependencies;
    }
}
