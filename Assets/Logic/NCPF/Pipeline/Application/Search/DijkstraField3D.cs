using Core.Foundation;
using NCPF.Domain;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// Heading-aware distance field over the GEOMETRIC lattice: a backward Dijkstra on
    /// (x, y, direction). It needs no velocity map — maxSpeed only converts the metric field into a
    /// time bound. Depends ONLY on the goal cell, so build it once and reuse it across ladder
    /// rungs/replans.
    /// </summary>
    public class DijkstraField3D
    {
        public readonly int W, H, A;
        public readonly Int2 GoalCell;
        public readonly Vector2 GoalPos;
        public readonly float MaxSpeed;
        public readonly int Pops;

        private readonly float[] _dist;

        public DijkstraField3D(IGridMap3D grid, int target, float maxSpeed, float turnPenaltyCells = 0.7f)
        {
            Config tc = grid.IdToCell3D(target);
            GoalCell = tc.Cell;
            GoalPos = grid.Cell3DToWorld(tc).Position;
            MaxSpeed = Mathf.Max(0.001f, maxSpeed);

            W = grid.Size.x;
            H = grid.Size.y;
            A = Mathf.Max(1, grid.AngleCount);
            _dist = Build(grid, GoalCell, turnPenaltyCells, out Pops);
        }

        public float Lookup(int x, int y, int a)
        {
            if (x < 0 || y < 0 || x >= W || y >= H || a < 0 || a >= A)
                return float.PositiveInfinity;
            return _dist[x + y * W + a * W * H];
        }

        private int Id(int x, int y, int a) => x + y * W + a * W * H;

        private float[] Build(IGridMap3D grid, Int2 goal, float turnPenaltyCells, out int pops)
        {
            int wh = W * H;
            float[] dist = new float[wh * A];
            for (int i = 0; i < dist.Length; i++)
                dist[i] = float.PositiveInfinity;

            pops = 0;
            if (goal.x < 0 || goal.y < 0 || goal.x >= W || goal.y >= H)
                return dist;

            float cs = grid.CellSize;
            float diag = cs * 1.41421356f;
            float turnCost = Mathf.Max(0f, turnPenaltyCells) * cs;

            int[] mvx = new int[A];
            int[] mvy = new int[A];
            for (int a = 0; a < A; a++)
            {
                float r = a * (2f * Mathf.PI / A);
                mvx[a] = Mathf.RoundToInt(-Mathf.Sin(r));
                mvy[a] = Mathf.RoundToInt(Mathf.Cos(r));
            }

            MinPriorityQueue open = new MinPriorityQueue();
            for (int a = 0; a < A; a++)
                if (grid.Cell3DFree(goal.x, goal.y, a))
                {
                    int id = Id(goal.x, goal.y, a);
                    dist[id] = 0f;
                    open.EnqueueOrDecrease(id, 0f);
                }

            while (open.Count > 0)
            {
                (int nodeId, float _) = open.Dequeue();
                pops++;
                float dc = dist[nodeId];
                int a = nodeId / wh;
                int rem = nodeId % wh;
                int y = rem / W;
                int x = rem % W;

                int aCcw = (a + 1) % A;
                int aCw = (a - 1 + A) % A;
                if (grid.Cell3DFree(x, y, aCcw)) Relax(dist, open, Id(x, y, aCcw), dc + turnCost);
                if (grid.Cell3DFree(x, y, aCw)) Relax(dist, open, Id(x, y, aCw), dc + turnCost);

                int dx = mvx[a], dy = mvy[a];
                if (dx == 0 && dy == 0) continue;
                float mcost = dc + ((dx != 0 && dy != 0) ? diag : cs);
                TryMove(grid, dist, open, x + dx, y + dy, a, mcost);
                TryMove(grid, dist, open, x - dx, y - dy, a, mcost);
            }
            return dist;
        }

        private void TryMove(IGridMap3D grid, float[] dist, MinPriorityQueue open, int nx, int ny, int a, float nd)
        {
            if (nx < 0 || ny < 0 || nx >= W || ny >= H) return;
            if (!grid.Cell3DFree(nx, ny, a)) return;
            Relax(dist, open, Id(nx, ny, a), nd);
        }

        private static void Relax(float[] dist, MinPriorityQueue open, int id, float nd)
        {
            if (nd < dist[id])
            {
                dist[id] = nd;
                open.EnqueueOrDecrease(id, nd);
            }
        }
    }
}
