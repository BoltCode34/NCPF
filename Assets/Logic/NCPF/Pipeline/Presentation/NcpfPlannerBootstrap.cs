using UnityEngine;
using UnityEngine.SceneManagement;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>
    /// Seeds the sharing cascade before any planner builds anything. A planner wires itself lazily —
    /// its Update does the building on the first tick — and AfterSceneLoad runs once the scene's
    /// objects exist but before that first tick, so this injects instead of overwriting: nothing has
    /// been built yet to throw away.
    /// The resolver stays alive so a planner instantiated later joins its group by being resolved,
    /// and is dropped on scene unload — otherwise the shared map, graph and neighbour cache would
    /// outlive the scene they describe.
    /// </summary>
    public static class NcpfPlannerBootstrap
    {
        private static MulPlannerDependencyResolver _resolver;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            _resolver = new MulPlannerDependencyResolver();
            SceneManager.sceneUnloaded -= Drop;
            SceneManager.sceneUnloaded += Drop;

            _resolver.ResolveAll(Object.FindObjectsByType<PathPlanner>(FindObjectsSortMode.None));
        }

        /// <summary>Joins a planner created after the initial pass — a spawned agent — to its group.</summary>
        public static void Resolve(PathPlanner planner)
        {
            _resolver ??= new MulPlannerDependencyResolver();
            _resolver.Resolve(planner);
        }

        private static void Drop(Scene scene)
        {
            _resolver = null;
        }
    }
}
