using NCPF.Domain;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// Turns a searched chain of <see cref="TransitionData"/> into real curves. The search names
    /// primitives by id and never touches a shape; this is where the ids become geometry — once, for
    /// the plan that won, instead of on every edge of every expansion. A primitive is baked at the
    /// origin pose, so reconstructing it is the library shape translated by the node the edge leaves
    /// from, which is why the edge carries its Start.
    /// </summary>
    public class PlanGeometry
    {
        private readonly DiscretizedControlSet3D _controlSet;
        private readonly IGridMap3D _grid;

        public PlanGeometry(DiscretizedControlSet3D controlSet, IGridMap3D grid)
        {
            _controlSet = controlSet;
            _grid = grid;
        }

        public CurvateTransition[] Resolve(TransitionData[] plan)
        {
            if (plan == null || plan.Length == 0)
            {
                return System.Array.Empty<CurvateTransition>();
            }

            UnityEngine.Vector2 origin = _grid.Cell3DToWorld(new Config(0, 0, 0)).Position;
            CurvateTransition[] geometry = new CurvateTransition[plan.Length];

            for (int i = 0; i < plan.Length; i++)
            {
                TransitionData edge = plan[i];
                DiscretizedPath3D primitive = _controlSet.GetById(edge.PrimitiveId);
                if (primitive == null)
                {
                    return System.Array.Empty<CurvateTransition>();
                }

                UnityEngine.Vector2 shift =
                    _grid.Cell3DToWorld(_grid.IdToCell3D(edge.Start)).Position - origin;

                geometry[i] = new CurvateTransition(
                    edge.Neightbor,
                    new PathOffset(primitive.Path, shift),
                    edge.Weight);
            }

            return geometry;
        }
    }
}
