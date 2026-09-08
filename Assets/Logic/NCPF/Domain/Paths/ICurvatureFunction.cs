namespace NCPF.Domain
{
    /// <summary>Curvature κ as a function of arc length over a span of length L.</summary>
    public interface ICurvatureFunction
    {
        float L { get; }

        float Evaluate(float s);
    }

    /// <summary>A curvature function that is a cubic polynomial in arc length.</summary>
    public interface ICubicPolynomialFunction : ICurvatureFunction
    {
        float A { get; }
        float B { get; }
        float C { get; }
        float D { get; }
        float MaxAbsCurvature { get; }
    }
}
