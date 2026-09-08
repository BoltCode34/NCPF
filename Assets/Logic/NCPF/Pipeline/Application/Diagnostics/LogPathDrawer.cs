using NCPF.Domain;
// DISABLED: this viewer targets the OLD 4D bake log (velocity in cells).
// The bake/log is geometric now (Config/WorldConfig). Re-enable by defining
// NCPF_LEGACY_DIAGNOSTICS after reworking it for the geometric log.
#if NCPF_LEGACY_DIAGNOSTICS
using Features.NCPF.Domain;
using UnityEngine;

public class LogPathDrawer : MonoBehaviour
{

    [SerializeField] private bool _rebuildPath = false;
    [SerializeField] private Config4D EndCell;
    public void OnDrawGizmos()
    {
        
    }
}

#endif
