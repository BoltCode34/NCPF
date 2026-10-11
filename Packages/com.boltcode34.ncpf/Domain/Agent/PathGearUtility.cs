using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// The gear of a primitive: +1 nose-first (tangent = heading), −1
    /// rear-first (tangent = heading + 180°). A rear-first primitive carries
    /// the <see cref="IRearFirstPath"/> marker, but it can be wrapped at any
    /// depth by decorators (<see cref="IPathWrapper"/>), so the check looks
    /// through the wrappers.
    /// </summary>
    public static class PathGearUtility
    {
        public const int Forward = 1;
        public const int Rear = -1;

        public static bool IsRearFirst(IPath path)
        {
            if (path == null)
            {
                return false;
            }
            if (path is IRearFirstPath)
            {
                return true;
            }
            if (path is IPathWrapper wrapper)
            {
                return IsRearFirst(wrapper.Inner);
            }
            return false;
        }

        public static int GearOf(IPath path) => IsRearFirst(path) ? Rear : Forward;

        /// <summary>
        /// The travel tangent at arc: the geometric nose heading, +180° on a
        /// rear-first primitive. Nose and tangent are two different channels:
        /// the primitive's angle channel speaks of the NOSE, while the body
        /// moves along the tangent.
        /// </summary>
        public static float TravelAngle(IPath path, float arc)
        {
            if (path == null)
            {
                return 0f;
            }
            float angle = path.Evaluate(arc).Angle;
            return IsRearFirst(path) ? Mathf.Repeat(angle + 180f, 360f) : angle;
        }
    }
}
