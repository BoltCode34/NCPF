namespace NCPF.Domain
{
    /// <summary>The 2D obstacle world the <see cref="MapBuilder"/> convolves into a <see cref="Map3D"/>.</summary>
    public class Map
    {
        public bool[,] Obstacles { get; private set; }
        public float CellSize;

        public Map(bool[,] obstacles, float cellSize)
        {
            Obstacles = obstacles;
            CellSize = cellSize;
        }
    }
}
