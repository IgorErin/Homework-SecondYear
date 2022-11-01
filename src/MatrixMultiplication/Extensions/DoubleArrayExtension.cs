namespace MatrixMultiplication.Extensions;

/// <summary>
/// Extension class for double.
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
           Math.Pow(Enumerable.Average(values), 2.0));
}
