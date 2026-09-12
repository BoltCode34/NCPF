using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>The geometric admission filter a layer builder prunes its candidates with.</summary>
    public interface IPathLimiter
    {
        public bool TryAllowPath(IPath path, IGridGeometry3D gridSpace, HashSet<Config> states);

        public bool CheckLength(IPath path);

        public bool CanBeDivided(IGridGeometry3D space,
            HashSet<Config> states,
            IPath path);
    }

    /// <summary>
    /// Geometric limiter: length ratio plus path decomposition. The
    /// equivalence tube is two independent absolute limits — position in world
    /// units, angle in degrees — instead of one grid-relative scale, and there
    /// is no kinematic check — feasibility w.r.t. speed is decided at search
    /// time.
    /// </summary>
    public class PathLimiterBase : IPathLimiter
    {
        private readonly float _breakDistance;
        private readonly float _positionTube;
        private readonly float _angleTube;
        private readonly float _minTurnRadius;

        public PathLimiterBase(float breakDistance = 1.4f, float positionTube = 0.25f, float angleTube = 10f, float minTurnRadius = 0f)
        {
            _breakDistance = breakDistance;
            _positionTube = Mathf.Max(0f, positionTube);
            _angleTube = Mathf.Max(0f, angleTube);
            _minTurnRadius = Mathf.Max(0f, minTurnRadius);
        }

        protected float PositionTube => _positionTube;
        protected float AngleTube => _angleTube;

        public virtual bool TryAllowPath(IPath path, IGridGeometry3D gridSpace, HashSet<Config> states)
        {
            return path != null &&
                CheckLength(path) &&
                CheckTurnRadius(path) &&
                !CanBeDivided(gridSpace, states, path);
        }

        public virtual bool CheckLength(IPath path)
        {
            float directDist = Vector2.Distance(path.Evaluate(0).Position, path.Evaluate(path.Length).Position);
            return path.Length <= (directDist * _breakDistance);
        }

        /// <summary>
        /// Reject primitives whose tightest turn is sharper than the agent's
        /// minimum turn radius. This is what actually bounds turn sharpness —
        /// the geometry, not the runtime heading channel. The tightest radius
        /// on the path is 1 / peak |dθ/ds|. 0 = disabled.
        /// </summary>
        public virtual bool CheckTurnRadius(IPath path)
        {
            if (_minTurnRadius <= 0f)
                return true;

            float length = path.Length;
            if (length <= 1e-4f)
                return true;

            const int samples = 24;
            float ds = length / samples;
            float prev = path.Evaluate(0f).Angle;
            float peakCurvature = 0f;
            for (int i = 1; i <= samples; i++)
            {
                float ang = path.Evaluate(i * ds).Angle;
                float k = Mathf.Abs(Mathf.DeltaAngle(prev, ang)) * Mathf.Deg2Rad / ds;
                if (k > peakCurvature) peakCurvature = k;
                prev = ang;
            }

            float minRadius = peakCurvature > 1e-6f ? 1f / peakCurvature : float.PositiveInfinity;
            return minRadius >= _minTurnRadius;
        }

        /// <summary>
        /// Sample density tracks path length against the position tube, not a
        /// fixed count — a fixed sample count spaces samples farther apart as
        /// primitives get longer, letting a long large-radius primitive thread
        /// between two samples and slip past a captured node's tube undetected.
        /// </summary>
        public virtual bool CanBeDivided(IGridGeometry3D space,
            HashSet<Config> states,
            IPath path)
        {
            float posTube = PositionTube;
            float angleTube = AngleTube;
            int samples = Mathf.Max(20, Mathf.CeilToInt(path.Length / Mathf.Max(posTube * 0.5f, 0.001f)));
            float step = path.Length / samples;

            Config start = space.WorldToCell3D(path.Evaluate(0));
            Config end = space.WorldToCell3D(path.Evaluate(path.Length));

            for (int i = 0; i <= samples; i++)
            {
                WorldConfig sample = path.Evaluate(step * i);
                Config cell = space.WorldToCell3D(sample);
                if (cell.Cell == start.Cell || cell.Cell == end.Cell)
                    continue;
                if (!states.Contains(cell))
                    continue;

                WorldConfig node = space.Cell3DToWorld(cell);
                if (Vector2.Distance(sample.Position, node.Position) < posTube &&
                    Mathf.Abs(Mathf.DeltaAngle(sample.Angle, node.Angle)) < angleTube)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
