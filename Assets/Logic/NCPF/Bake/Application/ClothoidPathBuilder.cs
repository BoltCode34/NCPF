using ClothoidX;
using NCPF.Domain;
using UnityEngine;

namespace NCPF.Bake.Application
{
    /// <summary>
    /// Builds a <see cref="ClothoidPath"/> between two poses by solving the G1
    /// Hermite interpolation problem (Bertolazzi–Frego). Pure geometry — the
    /// bake-side builder; dynamics live in the Pipeline assembly. NCPF measures
    /// direction from +Y (CCW), the clothoid solver from +X (CCW), so angles get
    /// +90° on the way in — and <see cref="ClothoidPath"/> takes it back off on
    /// the way out.
    /// </summary>
    public sealed class ClothoidPathBuilder : IPathBuilder
    {
        private const double NcpfToClothoidDeg = 90.0;

        public IPath Create(WorldConfig start, WorldConfig end)
        {
            double theta0 = (start.Angle + NcpfToClothoidDeg) * Mathf.Deg2Rad;
            double theta1 = (end.Angle + NcpfToClothoidDeg) * Mathf.Deg2Rad;

            ClothoidSegment segment = ClothoidSolutionBertolazziFrego.G1Segment(
                start.X, start.Y, theta0,
                end.X, end.Y, theta1, tol: 0.001);

            return new ClothoidPath(segment, start, end);
        }
    }
}
