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
    public interface ICurvateGraph : IGraph<TransitionData>
    {
    }

    /// <summary>
    /// Geometric edge: a shape reaching a 3D node. The shape is an <see cref="IPath"/>, not a
    /// <see cref="DiscretizedPath3D"/> — the cell sweep is a BAKE-time concern (collision) and the
    /// search never reads it, so carrying it would mean copying an offset cell array per edge per
    /// expansion and throwing it away unread.
    /// </summary>
    public struct CurvateTransition : ITransition
    {
        public IPath Path;
        public int Neightbor { get; set; }
        public float Weight { get; set; }

        public CurvateTransition(int neightbor, IPath path, float weight)
        {
            Neightbor = neightbor;
            Path = path;
            Weight = weight;
        }
    }
}
