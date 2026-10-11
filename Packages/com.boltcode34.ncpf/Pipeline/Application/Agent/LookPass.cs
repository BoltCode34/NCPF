using NCPF.Domain;
using UnityEngine;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// Turns the head toward whatever the look mode asks for, no faster than w_max. Position and
    /// speed are not its business: the main planner already computed them, and touching them would
    /// make its arithmetic a lie about how long the motion took. Swap the <see cref="IStateOverride"/>
    /// and the same pass aims at anything — the only thing that varies, so nothing above this needs
    /// its own pass type.
    /// </summary>
    public class LookPass : BaseAPass
    {
        private readonly IStateOverride _look;
        private readonly float _wMax;

        public LookPass(IStateOverride look, float wMaxDegPerSec)
        {
            _look = look;
            _wMax = Mathf.Max(1f, wMaxDegPerSec);
        }

        public override float OverrideAngle(float dt, DynamicState current, ControlInput input)
            => Mathf.MoveTowardsAngle(current.Angle, _look.OverrideState(current).Angle, _wMax * dt);
    }
}
