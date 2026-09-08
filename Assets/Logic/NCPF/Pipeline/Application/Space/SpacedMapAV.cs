using NCPF.Domain;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// The (angle, velocity) map over a planning window. A node is a <see cref="ConfigAV"/>:
    /// (sample along the window, facing bucket, speed bucket). The path is already chosen, so
    /// position is just an index; what is still open is the two channels the agent controls — and
    /// they cannot be opened separately: the turn allowed on a segment depends on how long you
    /// spend on it (|Δa| ≤ w_max · Δs/v). Slowing down BUYS turning. It carries no preferences: only
    /// what is physically possible and what it intrinsically costs; wanting to aim at something
    /// arrives as a decorator over its weights.
    /// </summary>
    public class SpacedMapAV : IGridMapAV
    {
        private const float EPS = 1e-4f;

        private readonly MotionWindow _window;
        private readonly IGridMap3D _map3D;
        private readonly int _angleCount, _velocityCount, _maxSpan;
        private readonly float _angleStep, _velocityStep, _wMax, _aMax;

        public SpacedMapAV(MotionWindow window, IGridMap3D map3D, int velocityCount, float vMax,
                           float wMaxDegPerSec, float aMax, int maxSpan)
        {
            _window = window;
            _map3D = map3D;
            _angleCount = Mathf.Max(1, map3D.AngleCount);
            _velocityCount = Mathf.Max(2, velocityCount);
            _angleStep = 360f / _angleCount;
            _velocityStep = Mathf.Max(0.001f, vMax) / (_velocityCount - 1);
            _wMax = Mathf.Max(1f, wMaxDegPerSec);
            _aMax = Mathf.Max(0.001f, aMax);
            _maxSpan = Mathf.Max(1, maxSpan);
        }

        public int SampleCount => _window.Count;
        public int AngleCount => _angleCount;
        public int VelocityCount => _velocityCount;
        public float AngleStep => _angleStep;
        public float VelocityStep => _velocityStep;

        public int CellAVToId(ConfigAV c) => (c.Sample * _angleCount + c.Angle) * _velocityCount + c.Velocity;

        public ConfigAV IdToCellAV(int id)
        {
            int v = id % _velocityCount;
            int rem = id / _velocityCount;
            return new ConfigAV(rem / _angleCount, rem % _angleCount, v);
        }

        public bool CellAVFree(ConfigAV c)
        {
            if (c.Sample < 0 || c.Sample >= _window.Count) return false;
            return _map3D.Cell3DFree(_window.Cx[c.Sample], _window.Cy[c.Sample], c.Angle);
        }

        public bool PointFree(int id) => CellAVFree(IdToCellAV(id));

        public WorldConfigAV CellAVToWorld(ConfigAV c)
            => new WorldConfigAV(c.Sample, c.Angle * _angleStep, c.Velocity * _velocityStep);

        public ConfigAV WorldToCellAV(WorldConfigAV c)
            => new ConfigAV(
                c.Sample,
                ((Mathf.RoundToInt(c.Angle / _angleStep) % _angleCount) + _angleCount) % _angleCount,
                Mathf.Clamp(Mathf.RoundToInt(c.Velocity / _velocityStep), 0, _velocityCount - 1));

        /// <summary>Nearest facing bucket the body actually fits in at that sample.</summary>
        public int NearestFreeAngle(int sample, int desired)
        {
            if (CellAVFree(new ConfigAV(sample, desired, 0))) return desired;
            for (int off = 1; off <= _angleCount / 2; off++)
            {
                int up = ((desired + off) % _angleCount + _angleCount) % _angleCount;
                int dn = ((desired - off) % _angleCount + _angleCount) % _angleCount;
                if (CellAVFree(new ConfigAV(sample, up, 0))) return up;
                if (CellAVFree(new ConfigAV(sample, dn, 0))) return dn;
            }
            return desired;
        }

        public DirectTransition[] GetNeightbors(int id)
        {
            ConfigAV from = IdToCellAV(id);
            float speed0 = from.Velocity * _velocityStep;

            List<DirectTransition> res = new List<DirectTransition>();

            for (int k = 1; k <= _maxSpan && from.Sample + k < _window.Count; k++)
            {
                int j = from.Sample + k;
                float ds = _window.Arc[j] - _window.Arc[from.Sample];
                if (ds < EPS) continue;

                for (int v2 = 0; v2 < _velocityCount; v2++)
                {
                    float speed1 = v2 * _velocityStep;

                    float vAvg = 0.5f * (speed0 + speed1);
                    if (vAvg < EPS) continue;

                    if (Mathf.Abs(speed1 * speed1 - speed0 * speed0) > 2f * _aMax * ds + EPS) continue;
                    float kappa = _window.Curvature[j];
                    if (kappa > EPS && speed1 > Mathf.Sqrt(_aMax / kappa) + 1e-3f) continue;

                    float dt = ds / vAvg;
                    float maxTurn = _wMax * dt;

                    for (int a2 = 0; a2 < _angleCount; a2++)
                    {
                        float turn = Mathf.Abs(Mathf.DeltaAngle(from.Angle * _angleStep, a2 * _angleStep));
                        if (turn > maxTurn) continue;
                        if (!SweepFree(from.Sample, j, from.Angle, a2)) continue;

                        res.Add(new DirectTransition(CellAVToId(new ConfigAV(j, a2, v2)), dt));
                    }
                }
            }
            return res.ToArray();
        }

        /// <summary>
        /// The body SWEEPS from (i, a) to (j, a2): it rotates through every intermediate sector while
        /// it slides through every intermediate cell. Neither is a node, so nothing else in the search
        /// can see them — without this the needle rotates straight through walls. The arc checked is
        /// the SHORTEST one (exactly the arc the runtime takes), sub-stepped at the finer of the two
        /// resolutions, so a fast wide turn cannot step over a blocked sector.
        /// </summary>
        private bool SweepFree(int i, int j, int a, int a2)
        {
            int diff = ((a2 - a) % _angleCount + _angleCount) % _angleCount;
            if (diff > _angleCount / 2) diff -= _angleCount;

            int span = j - i;
            int n = Mathf.Max(Mathf.Abs(diff), span);

            for (int m = 1; m <= n; m++)
            {
                float t = (float)m / n;
                int am = ((a + Mathf.RoundToInt(diff * t)) % _angleCount + _angleCount) % _angleCount;
                int s = i + Mathf.Min(span, Mathf.RoundToInt(span * t));
                if (!CellAVFree(new ConfigAV(s, am, 0)))
                    return false;
            }
            return true;
        }
    }
}
