using System.Collections.Generic;
using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// The agent MODEL: what the agent is, not what it decides. It holds the
    /// path parts, the cursor along them, and the current state — nothing else.
    /// Every decision (how fast, which way to look) belongs to the passes; the
    /// pipeline plans a state and writes it back here. The cursor is ARC LENGTH,
    /// not time: the path is pure geometry and carries no timing, so "where am
    /// I" can only be a distance.
    /// </summary>
    public class PathAgentModel
    {
        private readonly List<IPath> _parts = new();

        private int _partIndex;
        private float _arc;
        private DynamicState _state;

        public IReadOnlyList<IPath> Parts => _parts;
        public int CurrentPartIndex => _partIndex;
        public float CurrentPartArc => _arc;
        public IPath CurrentPart => _parts.Count > 0 ? _parts[_partIndex] : null;

        /// <summary>The state the pipeline last wrote. This is the agent's truth. Speed is
        /// signed: its sign is the GEAR of the current part (+ nose-first, − rear-first).</summary>
        public DynamicState CurrentDynamicState => _state;

        public bool Completed =>
            _parts.Count > 0 &&
            _partIndex == _parts.Count - 1 &&
            _arc >= _parts[_partIndex].Length - 1e-3f;

        public bool HasPath => _parts.Count > 0;

        /// <summary>
        /// Hot plan swap: new path, SAME cursor (part/arc) and state. The stitch
        /// delivers a result that begins with the current bridge primitive — the
        /// same object, — so the saved cursor stays valid untouched: no position
        /// projection needed (and a projection is a full sample sweep, expensive).
        /// Speed, nose and position survive the swap — no freeze, no teleport.
        /// </summary>
        public void ContinueWithStateSaving(IPath[] path)
        {
            if (path == null || path.Length == 0)
            {
                return;
            }

            _parts.Clear();
            _parts.AddRange(path);
            _partIndex = 0;
            _arc = Mathf.Min(_arc, _parts[0].Length);
        }

        /// <summary>
        /// Cold start: everything is cleared, the cursor begins at the path's
        /// start (the search was seeded from the body — the path's start IS the
        /// body). The seed is a STATE, not a vector: the nose stays as it is
        /// (the pass's rate-limit owns it), the speed carries over in the
        /// current gear — a gear flip is a cusp and dies to zero, a matching
        /// gear survives.
        /// </summary>
        public void StartFollowing(IPath[] path, DynamicState startPos)
        {
            _parts.Clear();
            _partIndex = 0;
            _arc = 0f;

            if (path == null || path.Length == 0)
            {
                _state = new DynamicState(startPos.Position, startPos.Angle, 0f, 0f);
                return;
            }

            _parts.AddRange(path);

            WorldConfig pose = _parts[0].Evaluate(0f);
            float speed = startPos.Speed * PathGearUtility.GearOf(_parts[0]) < 0f ? 0f : startPos.Speed;
            _state = new DynamicState(pose.Position, startPos.Angle, speed, PathGearUtility.TravelAngle(_parts[0], 0f));
        }

        /// <summary>The pipeline writes the planned state back. The model never decides it.</summary>
        public void Apply(DynamicState state) => _state = state;

        /// <summary>Move the cursor <paramref name="ds"/> metres along the path (clamped at the end)
        /// and return the raw GEOMETRIC pose there — no speed, no facing, those are planned.</summary>
        public WorldConfig AdvanceByArc(float ds)
        {
            if (_parts.Count == 0)
                return new WorldConfig(_state.Position, _state.Angle);

            float left = Mathf.Max(0f, ds);
            while (left > 0f)
            {
                float remaining = _parts[_partIndex].Length - _arc;

                if (remaining <= 1e-4f)
                {
                    if (_partIndex >= _parts.Count - 1)
                    {
                        _arc = _parts[_partIndex].Length; break;
                    }
                    _partIndex++;
                    _arc = 0f;
                    continue;
                }

                if (left < remaining)
                {
                    _arc += left;
                    left = 0f;
                }
                else
                {
                    left -= remaining;
                    if (_partIndex >= _parts.Count - 1)
                    {
                        _arc = _parts[_partIndex].Length;
                        break;
                    }
                    _partIndex++;
                    _arc = 0f;
                }
            }
            return _parts[_partIndex].Evaluate(_arc);
        }

        /// <summary>
        /// Samples the path ahead of the cursor WITHOUT moving it: a pose every
        /// <paramref name="dsStep"/> metres, up to <paramref name="maxMeters"/> of travel. This is
        /// the discretized window the planner searches over.
        /// </summary>
        public List<WorldConfig> PeekAheadArc(float dsStep, float maxMeters, int maxSamples = 128)
        {
            List<WorldConfig> result = new List<WorldConfig>();
            if (_parts.Count == 0) return result;

            dsStep = Mathf.Max(0.001f, dsStep);
            int pi = _partIndex;
            float a = _arc;

            WorldConfig prev = _parts[pi].Evaluate(a);
            result.Add(prev);

            float dist = 0f;
            bool ended = false;
            while (!ended && dist < maxMeters && result.Count < maxSamples)
            {
                float left = dsStep;
                while (left > 0f)
                {
                    float remaining = _parts[pi].Length - a;
                    if (remaining <= 1e-4f)
                    {
                        if (pi >= _parts.Count - 1)
                        {
                            ended = true;
                            break;
                        }
                        pi++;
                        a = 0f;
                        continue;
                    }

                    if (left < remaining)
                    {
                        a += left;
                        left = 0f;
                    }
                    else
                    {
                        left -= remaining;
                        if (pi >= _parts.Count - 1)
                        {
                            a = _parts[pi].Length;
                            ended = true;
                            break;
                        }
                        pi++;
                        a = 0f;
                    }
                }
                WorldConfig s = _parts[pi].Evaluate(a);
                dist += Vector2.Distance(prev.Position, s.Position);
                result.Add(s);
                prev = s;
            }
            return result;
        }
    }
}
