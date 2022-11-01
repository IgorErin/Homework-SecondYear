namespace MatrixMultiplication.Extensions;

/// <summary>
/// Extension class for int.
/// </summary>
public static class IntExtension
{
    /// <summary>
    /// Extension method for int raising it to an integer power.
    /// </summary>
    /// <param name="number">Number to be raised to a power.</param>
    /// <param name="powValue">The power to which the number will be raised.</param>
    /// <returns>Int value - result of exponentiation. </returns>
    public static int IntPow(this int number, int powValue)
        => (int)Math.Pow(number, powValue);
}
