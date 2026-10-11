using NCPF.Domain;
using UnityEngine;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>Wraps a <see cref="DynamicAgent"/> for editor authoring; the inner agent is rebuilt
    /// on each read so inspector tweaks apply immediately.</summary>
    [CreateAssetMenu(fileName = "DynamicAgentConfig", menuName = "NCPF/Configs/DynamicAgentConfig")]
    public class DynamicAgentConfig : ScriptableObject
    {
        [SerializeField] private DynamicAgent _agent;

        public DynamicAgent Agent => LoadAgent();

        public virtual DynamicAgent LoadAgent()
        {
            return new DynamicAgent(_agent.MaxLinearAcceleration, _agent.MaxVelocity, _agent.MaxAngledVelocity);
        }
    }
}
