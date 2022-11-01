namespace MatrixMultiplication.Extensions;

using Matrices;
using Strategies;

using System.Diagnostics;

/// <summary>
/// <see cref="Stopwatch"/> extensions class.
/// </summary>
public static class StopWatchExtensions
{
    /// <summary>
    /// Function to get time of mul execution.
    /// </summary>
    /// <param name="stopwatch"><see cref="Stopwatch"/> instance.</param>
    /// <param name="strategy">Strategy of multiplication.</param>
    /// <param name="leftMatrix">Array passed as left argument to function.</param>
    /// <param name="rightMatrix">Array passed as right argument to function.</param>
    /// <returns>The result of the passed function applied to the two passed arrays.</returns>
    public static long ResetAndGetTimeOfIntMatrixMultiplication(
        this Stopwatch stopwatch,
        IMultiplicationStrategy strategy,
        IntMatrix leftMatrix,
        IntMatrix rightMatrix)
    {
        stopwatch.Reset();

        stopwatch.Start();
        leftMatrix.MultiplyWithStrategy(rightMatrix, strategy);
        stopwatch.Stop();

        return stopwatch.ElapsedMilliseconds;
    }
}
