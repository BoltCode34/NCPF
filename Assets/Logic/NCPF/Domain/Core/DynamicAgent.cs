using System;

namespace NCPF.Domain
{
    /// <summary>
    /// Body limits of the moving agent: top speed, linear acceleration and turn rate.
    /// </summary>
    [Serializable]
    public class DynamicAgent
    {
        public float MaxVelocity;
        public float MaxLinearAcceleration;
        public float MaxAngledVelocity;
        public DynamicAgent(float acceleration, float maxVelocity, float maxAngledVelocity)
        {
            MaxLinearAcceleration = acceleration;
            MaxVelocity = maxVelocity;
            MaxAngledVelocity = maxAngledVelocity;
        }
    }
}
