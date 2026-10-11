using NCPF.Domain;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// The window the facing planner searches: the upcoming path, discretized into samples, with the
    /// geometry each sample needs. Built ONCE per replan and read by the map and the goal — otherwise
    /// each of them would re-sample the path and re-derive curvature on every node query, of which
    /// there are thousands per replan.
    /// </summary>
    public class MotionWindow
    {
        public readonly int Count;
        public readonly Vector2[] Pos;
        public readonly float[] Arc;
        public readonly float[] Curvature;
        public readonly int[] Cx, Cy;
        public readonly float[] Travel;
        public readonly float[] Aim;
        public float TotalArc => Count > 0 ? Arc[Count - 1] : 0f;

        public MotionWindow(IReadOnlyList<WorldConfig> poses, IGridMap3D grid, Vector2 target)
        {
            Count = poses.Count;
            Pos = new Vector2[Count];
            Arc = new float[Count];
            Curvature = new float[Count];
            Cx = new int[Count];
            Cy = new int[Count];
            Travel = new float[Count];
            Aim = new float[Count];

            for (int i = 0; i < Count; i++)
            {
                Pos[i] = poses[i].Position;
                Travel[i] = poses[i].Angle;

                Config c = grid.WorldToCell3D(poses[i]);
                Cx[i] = c.X;
                Cy[i] = c.Y;

                Vector2 d = target - Pos[i];
                Aim[i] = d.sqrMagnitude < 1e-6f ? poses[i].Angle
                                                : Mathf.Atan2(-d.x, d.y) * Mathf.Rad2Deg;

                if (i > 0) Arc[i] = Arc[i - 1] + Vector2.Distance(Pos[i - 1], Pos[i]);
            }

            for (int i = 1; i < Count - 1; i++)
            {
                float ds = Arc[i + 1] - Arc[i - 1];
                if (ds < 1e-4f) continue;
                float dTheta = Mathf.Abs(Mathf.DeltaAngle(Travel[i - 1], Travel[i + 1])) * Mathf.Deg2Rad;
                Curvature[i] = dTheta / ds;
            }
            if (Count > 2)
            {
                Curvature[0] = Curvature[1];
                Curvature[Count - 1] = Curvature[Count - 2];
            }
        }
    }

    /// <summary>Reached the end of the look-ahead window — there is no destination here, the path is
    /// already chosen; the search only has to live through the horizon, in any mode and at any speed.</summary>
    public class WindowEndAchivement : IAchivementHandler
    {
        private readonly FacingModeMap _map;
        private readonly int _last;

        public WindowEndAchivement(int sampleCount, FacingModeMap map)
        {
            _map = map;
            _last = sampleCount - 1;
        }

        public bool Achieved(int id)
        {
            _map.Decode(id, out int sample, out _, out _);
            return sample >= _last;
        }
    }

    /// <summary>No estimate: the remaining cost can be zero when the target can be faced all the way,
    /// so nothing larger stays admissible. The window is small enough for a plain Dijkstra.</summary>
    public class ZeroHeuristic : IHeuristicHandler
    {
        public float Heuristic(int id) => 0f;
    }
}
