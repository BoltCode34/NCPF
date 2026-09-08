using NCPF.Domain;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// The agent FOLLOWER and the loop itself: the model holds the state, the MAIN pass decides the
    /// control from it, optional passes amend that decision, and only then does the follower move
    /// the cursor and write the result back. Moving is NOT a pass's job — the move happens once,
    /// after everyone has spoken, so the last word holds. The planner hands in a GEOMETRIC path and
    /// never touches the follower otherwise.
    /// </summary>
    public class PathFollowAgent
    {
        private readonly PathAgentModel _model;
        private readonly MainSAPass _mainPlanner;
        public List<SAPass> Passes;

        public PathFollowAgent(PathAgentModel model, MainSAPass mainPlanner, params SAPass[] passes)
        {
            _model = model;
            _mainPlanner = mainPlanner;
            Passes = passes?.ToList() ?? new List<SAPass>();
        }

        public DynamicState GetNext(float dt)
        {
            DynamicState current = _model.CurrentDynamicState;

            ControlInput input = new ControlInput(current.Speed, current.Angle);
            input = _mainPlanner.PlanControl(dt, current);
            for (int i = 0; i < Passes.Count; i++)
                input = Passes[i].Override(dt, current, input);

            WorldConfig pose = _model.AdvanceByArc(Mathf.Abs(input.TargetSpeed) * dt);

            float velocityAngle = PathGearUtility.TravelAngle(_model.CurrentPart, _model.CurrentPartArc);
            DynamicState result = new DynamicState(pose.Position, input.TargetFacing, velocityAngle, input.TargetSpeed);
            _model.Apply(result);
            return result;
        }

        /// <summary>Takes the path as pure GEOMETRY — the model stores nothing else. The seed STATE
        /// carries position, nose and speed: on a cold start the planner passes the body's live
        /// state, and the model projects it onto the corridor.</summary>
        public void StartFollowing(CurvateTransition[] geometry, DynamicState state)
            => _model.StartFollowing(ToParts(geometry), state);

        /// <summary>Hot-swap the plan mid-motion: state (speed/nose) survives the path change.</summary>
        public void ContinueWithStateSaving(CurvateTransition[] geometry)
            => _model.ContinueWithStateSaving(ToParts(geometry));

        public bool HasPath => _model.HasPath;
        public bool Completed => _model.Completed;
        public DynamicState CurrentDynamicState => _model.CurrentDynamicState;
        public PathAgentModel Model => _model;

        /// <summary>Index of the primitive under the cursor — the seam needs the anchor: the end of
        /// exactly the current primitive, which the body is guaranteed to pass.</summary>
        public int CurrentPartIndex => _model.CurrentPartIndex;

        private static IPath[] ToParts(CurvateTransition[] path)
        {
            if (path == null) return System.Array.Empty<IPath>();
            IPath[] parts = new IPath[path.Length];
            for (int i = 0; i < path.Length; i++) parts[i] = path[i].Path;
            return parts;
        }
    }
}
