namespace MatrixMul.Extensions;

/// <summary>
/// Extension class for int
/// </summary>
public static class DoubleArrayExtensions
{
    /// <summary>
    /// Extension method calculus deviation.
    /// </summary>
    /// <param name="values">Values to calculate deviation.</param>
    /// <returns>Double value - result of exponentiation.</returns>
    public static double GetDeviation(this double[] values)
        => Math.Sqrt(Enumerable.Average(values.Select(x => x * x)) -
           Enumerable.Average(values) * Enumerable.Average(values));
}