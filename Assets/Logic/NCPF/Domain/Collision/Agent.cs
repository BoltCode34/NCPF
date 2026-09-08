using Core.Foundation;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// A movable body shape for map baking: position of its local origin on
    /// the world grid, heading, cell size, and the boolean footprint the world
    /// is convolved with. The anchor defaults to the footprint's filled centre.
    /// </summary>
    public abstract class Agent
    {
        public int X = 0;
        public int Y = 0;
        public float Angle = 0;
        public float CellSize = 1;

        protected Agent(float cellSize = 1)
        {
            CellSize = cellSize;
        }

        public Int2 Anchor => GetLocalAnchor();
        public bool[,] Footprint => GetFootPrint();

        public abstract bool[,] GetFootPrint();

        public virtual Int2 GetLocalAnchor()
        {
            bool[,] footprint = GetFootPrint();
            int width = footprint.GetLength(0);
            int height = footprint.GetLength(1);
            int minX = width - 1;
            int minY = height - 1;
            int maxX = 0;
            int maxY = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (footprint[x, y])
                    {
                        if (x < minX) minX = x;
                        if (y < minY) minY = y;
                        if (x > maxX) maxX = x;
                        if (y > maxY) maxY = y;
                    }
                }
            }
            int anchorX = minX + Mathf.RoundToInt((maxX - minX) / 2f);
            int anchorY = minY + Mathf.RoundToInt((maxY - minY) / 2f);
            Int2 anchor = new Int2(anchorX, anchorY);
            return anchor;
        }
    }
}
