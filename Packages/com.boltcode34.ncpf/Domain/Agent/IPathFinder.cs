namespace NCPF.Domain
{
    public interface IPathFinder
    {
        public int[] FindPath(IGraph graph, int from, int to);
    }
}