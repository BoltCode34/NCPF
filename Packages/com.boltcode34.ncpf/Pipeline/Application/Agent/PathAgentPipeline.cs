using NCPF.Domain;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// The planning BRAIN: it owns the geometric search and the path follower and runs the
    /// uninterrupted search loop — target aging, replan throttling (wall-clock outside play mode),
    /// waiting out an in-process search, consuming a buffered result with a stitched hot/cold
    /// plan swap, and feeding the follower. The shell hands in the body's live state each tick;
    /// the brain never touches a transform. The path cursor lives here.
    /// </summary>
    public class PathAgentPipeline
    {
        private readonly PathFindingAgent _search;
        private readonly PathFollowAgent _follower;
        private readonly IGridMap3D _map;
        private readonly ITarget _target;
        private readonly float _replanInterval;

        private CurvateTransition[] _path;
        private DynamicState? _rebuildSeed;
        private float _lastPlanTime = float.NegativeInfinity;

        public PathAgentPipeline(PathFindingAgent search, PathFollowAgent follower, IGridMap3D map,
            ITarget target, float replanInterval, DynamicState? rebuildSeed)
        {
            _search = search;
            _follower = follower;
            _map = map;
            _target = target;
            _replanInterval = replanInterval;
            _rebuildSeed = rebuildSeed;
        }

        public PathFollowAgent Follower => _follower;
        public CurvateTransition[] Path => _path;
        public DynamicState? RebuildSeed { get => _rebuildSeed; set => _rebuildSeed = value; }

        /// <summary>One tick of the search loop. The body state is the live pose; on a cold plan
        /// swap it doubles as the seed (or yields to a rebuild seed captured across a map rebuild).</summary>
        public void Update(DynamicState bodyState)
        {
            if (_search == null || _map == null || _target == null) return;

            bool stale = _path == null || _path.Length == 0 ||
                Vector2.Distance(_path[_path.Length - 1].Path.End.Position, _target.Position) > _map.CellSize * 2;
            if (!stale) return;
            if (_search.InProcess) return;

            if (_search.HasBufferedPath)
            {
                SetUpPath(bodyState);
                return;
            }

            float now = UnityEngine.Application.isPlaying ? Time.time : Time.realtimeSinceStartup;
            if (now - _lastPlanTime >= _replanInterval)
            {
                DynamicState end = new DynamicState(_target.Position, 0f, 0f, 0f);
                _search.BuildPath(bodyState, end);
                _lastPlanTime = now;
            }
        }

        /// <summary>Takes the buffered search result, stitches it to the current cursor and hands
        /// the geometry to the follower. Hot swap keeps the speed/nose; cold start seeds from the
        /// body state (or a rebuild seed that survived a map rebuild).</summary>
        private void SetUpPath(DynamicState bodyState)
        {
            CurvateTransition[] transitions = _search.StitchPath(_path, _follower.CurrentPartIndex);
            if (transitions == null || transitions.Length == 0) return;

            _path = transitions;
            if (_follower.HasPath)
            {
                _follower.ContinueWithStateSaving(transitions);
                return;
            }

            DynamicState seed = _rebuildSeed ?? bodyState;
            _rebuildSeed = null;
            _follower.StartFollowing(transitions, seed);
        }

        /// <summary>Releases the in-flight search and drops the plan — the next Update rebuilds it.</summary>
        public void Break()
        {
            _search?.Break();
            _path = null;
        }
    }
}
