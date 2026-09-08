using NCPF.Domain;

namespace NCPF.Pipeline.Application
{
    /// <summary>
    /// What the agent DECIDES this frame — the control input, not the state. There is no position
    /// here: the arc cursor inside <see cref="PathAgentModel"/> is the only thing that says where the
    /// body is, moved by the pipeline alone. Both channels are TARGETS for this frame, already
    /// rate-limited by whoever produced them — the pipeline applies them as-is.
    /// </summary>
    public struct ControlInput
    {
        /// <summary>Signed metres per second along the path: the sign is the gear (+ nose-first,
        /// − rear-first), the magnitude is the cursor's pace. The pipeline advances the cursor by |v|·dt.</summary>
        public float TargetSpeed;

        /// <summary>Where the body points, in NCPF degrees (0 = +Y, +CCW). Independent of travel
        /// direction — the agent is holonomic, it may look one way and fly another.</summary>
        public float TargetFacing;

        public ControlInput(float speed, float facing)
        {
            TargetSpeed = speed;
            TargetFacing = facing;
        }
    }

    /// <summary>
    /// One step of the control loop: read the state, return the input. The state comes in READ-ONLY
    /// (a pass needs the position to aim at something), and only a <see cref="ControlInput"/> goes
    /// out. That asymmetry is the contract: passes decide, the pipeline moves.
    /// </summary>
    public abstract class SAPass
    {
        /// <param name="current">The agent's truth, as the model last recorded it.</param>
        /// <param name="input">What the passes before this one decided. The main planner gets the
        /// current state's own speed and facing here.</param>
        public abstract ControlInput Override(float dt, DynamicState current, ControlInput input);
    }

    /// <summary>The MAIN pass — decides facing and speed together from the state alone.</summary>
    public abstract class MainSAPass
    {
        public abstract ControlInput PlanControl(float dt, DynamicState current);
    }

    /// <summary>
    /// The FACING channel. Leaves the speed exactly as it came in — the pass that set it computed
    /// the turn budget against that speed, and quietly changing it would make that arithmetic wrong.
    /// </summary>
    public abstract class BaseAPass : SAPass
    {
        public override ControlInput Override(float dt, DynamicState current, ControlInput input)
            => new ControlInput
            (
                input.TargetSpeed,
                OverrideAngle(dt, current, input)
            );

        public abstract float OverrideAngle(float dt, DynamicState current, ControlInput input);
    }

    /// <summary>
    /// The SPEED channel. The planner's path is pure geometry — it carries no timing — so a pass on
    /// this channel decides how fast the body travels it, and the pipeline then advances the arc
    /// cursor by <c>v·dt</c> using whatever the LAST pass left here.
    /// </summary>
    public abstract class BaseSPass : SAPass
    {
        public override ControlInput Override(float dt, DynamicState current, ControlInput input)
            => new ControlInput
            (
                OverrideSpeed(dt, current, input),
                input.TargetFacing
            );

        public abstract float OverrideSpeed(float dt, DynamicState current, ControlInput input);
    }
}
