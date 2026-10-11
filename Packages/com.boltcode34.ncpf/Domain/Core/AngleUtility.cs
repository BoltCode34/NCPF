using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// Angle helpers for the NCPF convention: degrees, 0 = +Y, positive counter-clockwise.
    /// AngleToDirection(a) = (-sin a, cos a). DirectionToAngle is NOT its exact inverse
    /// (historic sign bug); the correct inverse is Atan2(-dir.x, dir.y) in degrees.
    /// </summary>
    public static class AngleUtility
    {
        public static Vector2 AngleToDirection(float angle)
        {
            return Quaternion.Euler(0f, 0f, angle) * Vector2.up;
        }

        public static float DirectionToAngle(Vector2 dir)
        {
            dir.Normalize();

            return Mathf.Atan2(dir.x, dir.y)
                * Mathf.Rad2Deg;
        }
    }
}
