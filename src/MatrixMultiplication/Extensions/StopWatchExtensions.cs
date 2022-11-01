namespace MatrixMultiplication.Extensions;

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
    /// <param name="mulFunc">Function whose execution time will be calculated.</param>
    /// <param name="left2DArray">Array passed as left argument to function.</param>
    /// <param name="right2DArray">Array passed as right argument to function.</param>
    /// <returns>The result of the passed function applied to the two passed arrays.</returns>
    public static long ResetAndGetTimeOfMult(
        this Stopwatch stopwatch,
        Func<int[,], int[,], int[,]> mulFunc,
        int[,] left2DArray,
        int[,] right2DArray)
    {
        stopwatch.Reset();

        stopwatch.Start();
        mulFunc(left2DArray, right2DArray);
        stopwatch.Stop();

        return stopwatch.ElapsedMilliseconds;
    }
}
