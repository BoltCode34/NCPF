namespace NCPF.Domain
{
    /// <summary>
    /// One part of a <see cref="PrimitiveWindow"/>: a primitive with its
    /// dynamics precomputed. <see cref="Length"/> is the primitive's FULL
    /// remainder, <see cref="KappaPeak"/> the peak tangent curvature over that
    /// span (rad/m), <see cref="VCap"/> the speed ceiling from all three
    /// DynamicAgent limits.
    /// </summary>
    public class WindowPart
    {
        public readonly float Length;
        public readonly int Gear;
        public readonly float KappaPeak;
        public readonly float VCap;

        public WindowPart(float length, int gear, float kappaPeak, float vCap)
        {
            Length = length;
            Gear = gear;
            KappaPeak = kappaPeak;
            VCap = vCap;
        }
    }
}
