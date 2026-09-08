using Core.Foundation;

namespace NCPF.Domain
{
    public interface IGraphPathFinder
    {
        public Int2[] FindPath(IGrid grid, IGraph graph, Int2 from, Int2 to);
    }
}