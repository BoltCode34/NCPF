using NCPF.Shared.Presentation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// Seeds the sharing cascade before any planner builds anything. A planner that has no context
    /// wires itself in Start, and AfterSceneLoad runs after the scene's Awake calls but before any
    /// Start, so this injects instead of overwriting: nothing has been built yet to throw away.
    /// The resolver stays alive so a planner instantiated later joins its group by being resolved,
    /// and is dropped on scene unload — otherwise the shared map, graph and neighbour cache would
    /// outlive the scene they describe.
    /// Sharing is switched by <see cref="NcpfSettingsAsset.CombineDependencies"/>; with no settings
    /// asset in a Resources folder it stays on.
    /// </summary>
    public static class NcpfPlannerBootstrap
    {
        private const string SettingsResource = "NCPFSettings";

        private static MulPlannerDependencyResolver _resolver;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            SceneManager.sceneUnloaded -= Drop;
            SceneManager.sceneUnloaded += Drop;

            if (!CombineDependencies())
            {
                _resolver = null;
                return;
            }

            _resolver = new MulPlannerDependencyResolver();
            _resolver.ResolveAll(Object.FindObjectsByType<PathPlanner>(FindObjectsSortMode.None));
        }

        /// <summary>
        /// Joins a planner created after the initial pass — a spawned agent — to its group. Does
        /// nothing when sharing is off: the planner then wires itself on its first Update.
        /// </summary>
        public static void Resolve(PathPlanner planner)
        {
            if (!CombineDependencies())
            {
                return;
            }

            _resolver ??= new MulPlannerDependencyResolver();
            _resolver.Resolve(planner);
        }

        private static bool CombineDependencies()
        {
            NcpfSettingsAsset settings = Resources.Load<NcpfSettingsAsset>(SettingsResource);
            return settings == null || settings.CombineDependencies;
        }

        private static void Drop(Scene scene)
        {
            _resolver = null;
        }
    }
}
