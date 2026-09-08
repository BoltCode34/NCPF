using NCPF.Domain;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// The GEOMETRIC search graph: nodes are (x, y, direction), edges carry a pure shape.
    /// No speed, no time — dynamics are layered on after the search. Describing the graph —
    /// INCLUDING the weights of outgoing transitions — is the graph's own duty, which is exactly
    /// what lets graph DECORATORS (discounts, penalties) modify those weights. A concrete graph
    /// may compute weights itself or delegate the handling (CurvateMap3D delegates it to its
    /// ITransitionGenerator).
    /// </summary>
    public interface ICurvateGraph : IGraph<CurvateTransition>
    {
    }

    /// <summary>Geometric edge: a shape (<see cref="DiscretizedPath3D"/>) reaching a 3D node.</summary>
    public struct CurvateTransition : ITransition
    {
        public DiscretizedPath3D Path;
        public int Neightbor { get; set; }
        public float Weight { get; set; }

        public CurvateTransition(int neightbor, DiscretizedPath3D path, float weight)
        {
            Neightbor = neightbor;
            Path = path;
            Weight = weight;
        }
    }
}
