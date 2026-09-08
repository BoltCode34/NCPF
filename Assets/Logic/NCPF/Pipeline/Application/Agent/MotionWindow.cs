using NCPF.Domain;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// The window the motion planner searches: the upcoming path, discretized into samples, with the
    /// geometry each sample needs. Built ONCE per frame and read by the map, the rules and the
    /// heuristic — otherwise each of them would re-sample the path and re-derive curvature on every
    /// node query, of which there are thousands per frame.
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
    /// already chosen; the search only has to live through the horizon, at any facing and speed.</summary>
    public class WindowEndAchivement : IAchivementHandler
    {
        private readonly IGridMapAV _map;
        private readonly int _last;

        public WindowEndAchivement(int sampleCount, IGridMapAV map)
        {
            _map = map;
            _last = sampleCount - 1;
        }

        public bool Achieved(int id) => _map.IdToCellAV(id).Sample >= _last;
    }

    /// <summary>
    /// THE RULES: looking AT THE PREY → speed pulled toward the SLOW target; looking ALONG THE PATH →
    /// speed pulled toward the FAST target. Two wells, the node pays the CHEAPER one (<c>min</c>) — a
    /// ridge between them, so resting half-way is never optimal and the agent commits to a mode
    /// instead of averaging them. <c>_travelBias</c> is how much aiming is preferred when both are
    /// available. It only PRICES a node, reading it THROUGH the map — packing/discretization is none
    /// of its business; getting the price into the plan is <see cref="MotionRulesGraph"/>'s job.
    /// </summary>
    public class MotionRules : ICostHandler
    {
        private readonly MotionWindow _w;
        private readonly IGridMapAV _map;
        private readonly float _slowSpeed, _fastSpeed;
        private readonly float _angleWeight, _speedWeight, _travelBias;

        public MotionRules(MotionWindow window, IGridMapAV map,
                           float slowSpeed, float fastSpeed,
                           float angleWeight, float speedWeight, float travelBias)
        {
            _w = window;
            _map = map;
            _slowSpeed = slowSpeed;
            _fastSpeed = fastSpeed;
            _angleWeight = angleWeight;
            _speedWeight = speedWeight;
            _travelBias = travelBias;
        }

        public float Cost(int id)
        {
            ConfigAV cell = _map.IdToCellAV(id);
            if (cell.Sample < 0 || cell.Sample >= _w.Count) return 0f;

            WorldConfigAV s = _map.CellAVToWorld(cell);

            float aimCost = _angleWeight * Mathf.Abs(Mathf.DeltaAngle(s.Angle, _w.Aim[cell.Sample]))
                          + _speedWeight * Mathf.Abs(s.Velocity - _slowSpeed);

            float travelCost = _travelBias
                             + _angleWeight * Mathf.Abs(Mathf.DeltaAngle(s.Angle, _w.Travel[cell.Sample]))
                             + _speedWeight * Mathf.Abs(s.Velocity - _fastSpeed);

            return Mathf.Min(aimCost, travelCost);
        }
    }

    /// <summary>
    /// The rules, delivered as WEIGHT — weight is the map's job, and moving it into the goal would be
    /// the same thing wearing a different hat. The inner map stays blind: it only knows what is
    /// physically possible and what it costs in time. It must not know that anyone wants to aim at a
    /// prey. A DECORATOR of it is exactly the place that may — that is what a decorator is for. Swap
    /// this one and the same lattice plans a completely different behaviour.
    /// </summary>
    public class MotionRulesGraph : IGraph<DirectTransition>
    {
        private readonly IGraph<DirectTransition> _inner;
        private readonly ICostHandler _rules;

        public MotionRulesGraph(IGraph<DirectTransition> inner, ICostHandler rules)
        {
            _inner = inner;
            _rules = rules;
        }

        public bool PointFree(int id) => _inner.PointFree(id);

        public DirectTransition[] GetNeightbors(int id)
        {
            DirectTransition[] edges = _inner.GetNeightbors(id);
            for (int i = 0; i < edges.Length; i++)
                edges[i].Weight += _rules.Cost(edges[i].Neightbor);
            return edges;
        }
    }

    /// <summary>Admissible lower bound on the remaining TIME (the map's intrinsic cost): the arc still
    /// to cover at the best possible speed. The rules are ≥ 0, so leaving them out keeps this
    /// optimistic — exactly what A* needs to stay honest.</summary>
    public class WindowTimeHeuristic : IHeuristicHandler
    {
        private readonly MotionWindow _w;
        private readonly IGridMapAV _map;
        private readonly float _vMax;

        public WindowTimeHeuristic(MotionWindow window, IGridMapAV map, float vMax)
        {
            _w = window;
            _map = map;
            _vMax = Mathf.Max(0.001f, vMax);
        }

        public float Heuristic(int id)
        {
            int i = _map.IdToCellAV(id).Sample;
            if (i < 0 || i >= _w.Count) return 0f;
            return (_w.TotalArc - _w.Arc[i]) / _vMax;
        }
    }
}
