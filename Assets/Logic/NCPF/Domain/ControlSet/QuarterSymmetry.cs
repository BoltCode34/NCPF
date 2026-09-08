using UnityEngine;

namespace NCPF.Domain
{
    /// <summary>
    /// Quarter-turn (90° CCW) symmetry of the geometric control set. A
    /// curvature-defined primitive is orientation-invariant, so only a quarter
    /// of the lattice is baked; the other three quadrants are rebuilt natively
    /// — same shape function, rotated start and end cells — instead of being
    /// wrapped in a rotation decorator. NCPF direction is 0 = +Y, +CCW, so one
    /// quarter-turn maps a cell (x, y) to (−y, x).
    /// </summary>
    public static class QuarterSymmetry
    {
        /// <summary>Number of angle layers in a full turn, derived from the grid.</summary>
        public static int FullAngleCount(IGridGeometry3D space)
            => Mathf.Max(4, Mathf.RoundToInt(360f / space.AngleStep));

        /// <summary>
        /// Rotate a 3D lattice cell by <paramref name="quarter"/> quarter-turns
        /// (+90° CCW) about the origin.
        /// </summary>
        public static Config RotateCell(Config cell, int quarter, int fullAngleCount)
        {
            int q = ((quarter % 4) + 4) % 4;

            int x = cell.X;
            int y = cell.Y;
            for (int i = 0; i < q; i++)
            {
                int rotatedX = -y;
                y = x;
                x = rotatedX;
            }

            int quarterAngle = fullAngleCount / 4;
            int angle = (cell.Angle + q * quarterAngle) % fullAngleCount;
            if (angle < 0) angle += fullAngleCount;

            return new Config(x, y, angle);
        }
    }
}
