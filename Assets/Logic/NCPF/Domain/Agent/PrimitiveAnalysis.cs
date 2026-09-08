namespace NCPF.Domain
{
    /// <summary>
    /// The analysis result of a primitive — pure geometry, no dynamics: what
    /// can be computed from the SHAPE and baked once for all projections. The
    /// analysis is projection-invariant (translation, rotation, gear), so one
    /// analysis answers one library entry, not each projected instance.
    /// </summary>
    public readonly struct PrimitiveAnalysis
    {
        /// <summary>The peak of |κ| over [0, Length], rad/m.</summary>
        public readonly float KappaPeak;

        public PrimitiveAnalysis(float kappaPeak)
        {
            KappaPeak = kappaPeak;
        }
    }
}
