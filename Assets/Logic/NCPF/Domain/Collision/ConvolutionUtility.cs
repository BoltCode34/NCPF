using Core.Foundation;

namespace NCPF.Domain
{
    /// <summary>
    /// Convolves a boolean kernel (the agent footprint) over a boolean world:
    /// a cell is free only where the kernel anchored there fits without
    /// overlapping an obstacle.
    /// </summary>
    public static class ConvolutionUtility
    {
        public static bool[,] Convolve(bool[,] world, bool[,] kernel, Int2 anchor)
        {
            bool[,] result = new bool[world.GetLength(0), world.GetLength(1)];
            for (int x = 0; x < world.GetLength(0); x++)
            {
                for (int y = 0; y < world.GetLength(1); y++)
                {
                    result[x, y] = ConvolvePos(world, kernel, anchor, new(x, y));
                }
            }
            return result;
        }

        private static bool ConvolvePos(bool[,] world, bool[,] kernel, Int2 anchor, Int2 pos)
        {
            Int2 start = pos - anchor;
            Int2 end = start + new Int2(kernel.GetLength(0), kernel.GetLength(1));
            bool result = true;
            int mapSizeX = world.GetLength(0);
            int mapSizeY = world.GetLength(1);
            for (int x = 0; x < kernel.GetLength(0); x++)
            {
                for (int y = 0; y < kernel.GetLength(1); y++)
                {
                    int worldPosX = start.x + x;
                    int worldPosY = start.y + y;
                    if (worldPosX < 0 || worldPosX >= mapSizeX || worldPosY < 0 || worldPosY >= mapSizeY)
                    {
                        continue;
                    }
                    bool kernelValue = kernel[x, y];
                    bool worldValue = world[worldPosX, worldPosY];
                    if (kernelValue && worldValue) return false;
                }
            }
            return result;
        }
    }
}
