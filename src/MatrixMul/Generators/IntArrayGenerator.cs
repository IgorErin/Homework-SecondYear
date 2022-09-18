using System;

namespace MatrixMul.Generators;

/// <summary>
/// Static class.
/// Int array generator.
/// </summary>
public static class IntArrayGenerator
{
    /// <summary>
    /// Max rand value of array items.
    /// </summary>
    private const int MaxRandValue = 20;

    /// <summary>
    /// A method that generates a two-dimensional array according to the given dimensions.
    /// </summary>
    /// <param name="rowCount">Row array count</param>
    /// <param name="columnCount">Column array count</param>
    /// <returns>Generated 2D array</returns>
    public static int[,] Generate2DIntArray(int rowCount, int columnCount)
    {
        var random = new Random();
        var new2DArray = new int[rowCount, columnCount];

        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                new2DArray[rowIndex, columnIndex] = random.Next(MaxRandValue);
            }
        }

        return new2DArray;
    }
}