namespace ClothoidX
{
    /// <summary>
    /// Describes the type of solution used in the ClothoidX project.
    /// This is used because some solutions come with different ways to draw the curve.
    /// The most accurate one is by far Bertolazzi-Frego, and it is the default one. But
    /// it is not compatible with some solutions. For example, the Singh-McCrae solution
    /// downsamples the resulting arc length / curvature graph and thus to fit the local
    /// space curve to the world space polyline, we need a center of mass calculation as 
    /// well as a rotation matrix.
    /// </summary>
    public enum SolutionType
    {
        SINGHMCCRAE = 0,
        BERTOLAZZIFREGO = 1,
    }
}