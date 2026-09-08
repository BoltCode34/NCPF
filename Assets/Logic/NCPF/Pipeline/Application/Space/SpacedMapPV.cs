using NCPF.Domain;
using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// The (part, velocity) map over the primitives window — the PV twin of <see cref="SpacedMapAV"/>.
    /// A node is a <see cref="ConfigPV"/>: (window part entry, SIGNED speed bucket; the sign is the
    /// gear). The path is already chosen and collision-checked by the 3D search, so there is no
    /// collision dimension here — <see cref="PointFree"/> is a bounds check only. After the geometry
    /// a single value is still open — the speed — and it is searched in ONE dimension. CUSP: the
    /// node (k+1, b) is both the exit of part k and the entry of part k+1; different gears intersect
    /// only at zero, so every gear change passes through a full stop — exactly the body's physics.
    /// </summary>
    public class SpacedMapPV : IGraph<DirectTransition>
    {
        private const float EPS = 1e-4f;

        private readonly PrimitiveWindow _window;
        private readonly int _speedSteps;
        private readonly float _speedStep;
        private readonly float _aMax;
        private readonly float _vTop;
        private readonly float _speedStrive;

        /// <summary>speedStrive is the MISSION "strive toward the ladder top" addend (s/m): the edge
        /// time gains a per-metre penalty weighted by the exit speed's share below the top. 0 is
        /// neutral; the weight only grows, so the time heuristic stays admissible.</summary>
        public SpacedMapPV(PrimitiveWindow window, float vTop, float aMax, int speedSteps, float speedStrive = 0f)
        {
            _window = window;
            _speedSteps = Mathf.Max(1, speedSteps);
            _speedStep = Mathf.Max(0.001f, vTop) / _speedSteps;
            _aMax = Mathf.Max(0.001f, aMax);
            _vTop = Mathf.Max(0.001f, vTop);
            _speedStrive = Mathf.Max(0f, speedStrive);
        }

        public int PartCount => _window.Count;
        public int SpeedSteps => _speedSteps;
        public int BucketCount => 2 * _speedSteps + 1;
        public float SpeedStep => _speedStep;

        public int CellPVToId(ConfigPV c) => c.Part * BucketCount + (c.Speed + _speedSteps);

        public ConfigPV IdToCellPV(int id)
        {
            int speed = id % BucketCount - _speedSteps;
            return new ConfigPV(id / BucketCount, speed);
        }

        public float BucketSpeed(int bucket) => bucket * _speedStep;

        public int SpeedToBucket(float speed)
            => Mathf.Clamp(Mathf.RoundToInt(speed / _speedStep), -_speedSteps, _speedSteps);

        public bool PointFree(int id)
        {
            ConfigPV c = IdToCellPV(id);
            if (c.Part < 0 || c.Part > _window.Count)
            {
                return false;
            }
            return c.Speed >= -_speedSteps && c.Speed <= _speedSteps;
        }

        public DirectTransition[] GetNeightbors(int id)
        {
            ConfigPV from = IdToCellPV(id);

            if (from.Part >= _window.Count)
            {
                return System.Array.Empty<DirectTransition>();
            }

            WindowPart part = _window.Parts[from.Part];
            if (from.Speed != 0 && from.Speed * part.Gear < 0)
            {
                return System.Array.Empty<DirectTransition>();
            }

            float v0 = from.Speed * _speedStep;
            List<DirectTransition> res = new List<DirectTransition>();

            for (int b1 = -_speedSteps; b1 <= _speedSteps; b1++)
            {
                if (b1 != 0 && b1 * part.Gear < 0)
                {
                    continue;
                }

                float v1 = b1 * _speedStep;
                float vAvg = 0.5f * (Mathf.Abs(v0) + Mathf.Abs(v1));
                if (vAvg < EPS)
                {
                    continue;
                }

                float cap = from.Part + 1 < _window.Count
                    ? Mathf.Min(part.VCap, _window.Parts[from.Part + 1].VCap)
                    : part.VCap;
                if (Mathf.Abs(v1) > cap + 1e-3f)
                {
                    continue;
                }

                if (Mathf.Abs(v1 * v1 - v0 * v0) > 2f * _aMax * part.Length + EPS)
                {
                    continue;
                }

                float weight = part.Length / vAvg;
                if (_speedStrive > 0f)
                {
                    weight += _speedStrive * part.Length * (_vTop - Mathf.Abs(v1)) / _vTop;
                }

                res.Add(new DirectTransition(
                    CellPVToId(new ConfigPV(from.Part + 1, b1)),
                    weight));
            }
            return res.ToArray();
        }
    }

    /// <summary>Live through to the window's end — there is no destination here, the path is already
    /// chosen; the speed plan only has to survive the horizon. Mirror of WindowEndAchivement.</summary>
    public class PrimitiveEndAchivement : IAchivementHandler
    {
        private readonly SpacedMapPV _map;
        private readonly int _last;

        public PrimitiveEndAchivement(SpacedMapPV map)
        {
            _map = map;
            _last = map.PartCount;
        }

        public bool Achieved(int id) => _map.IdToCellPV(id).Part >= _last;
    }

    /// <summary>An admissible lower bound on the remaining TIME: the arc to the window's end at the
    /// RAW v_max. Mirror of WindowTimeHeuristic.</summary>
    public class PrimitiveTimeHeuristic : IHeuristicHandler
    {
        private readonly PrimitiveWindow _window;
        private readonly SpacedMapPV _map;
        private readonly float _vMax;

        public PrimitiveTimeHeuristic(PrimitiveWindow window, SpacedMapPV map, float vMax)
        {
            _window = window;
            _map = map;
            _vMax = Mathf.Max(0.001f, vMax);
        }

        public float Heuristic(int id)
            => _window.RemainingArc(_map.IdToCellPV(id).Part) / _vMax;
    }
}
