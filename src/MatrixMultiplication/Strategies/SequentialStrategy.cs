namespace MatrixMultiplication.Strategies;

using MatrixExceptions;

/// <summary>
/// Class that implements the parallel multiplication strategy.
/// </summary>
public class SequentialStrategy : IMultiplicationStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SequentialStrategy"/> class.
    /// </summary>
    public SequentialStrategy()
    {
    }
    
    /// <summary>
    /// Sequential multiplication method.
    /// </summary>
    /// <param name="leftMatrix">left matrix array representation.</param>
    /// <param name="rightMatrix">right matrix array representation.</param>
    /// <returns>Result of multiplication.</returns>
    /// <exception cref="ArgumentException">Was thrown when dimensions did not match for multiplication.</exception>
    public int[,] Multiply(int[,] leftMatrix, int[,] rightMatrix)
    {
        var leftRowCount = leftMatrix.GetLength(0);
        var leftColumnCount = leftMatrix.GetLength(1);

        var rightRowCount = rightMatrix.GetLength(0);
        var rightColumnCount = rightMatrix.GetLength(1);

        if (leftColumnCount != rightRowCount)
        {
            throw new IntMatrixMultiplicationException(
                $"matrix multiplication is not possible, wrong dimension: {leftColumnCount} != {rightRowCount}");
        }

        var result = new int[leftRowCount, rightColumnCount];

        for (var leftRowIndex = 0; leftRowIndex < leftRowCount; leftRowIndex++)
        {
            for (var rightColumnIndex = 0; rightColumnIndex < rightColumnCount; rightColumnIndex++)
            {
                for (var currentItemIndex = 0; currentItemIndex < leftColumnCount; currentItemIndex++)
                {
                    result[leftRowIndex, rightColumnIndex] +=
                        leftMatrix[leftRowIndex, currentItemIndex] * rightMatrix[currentItemIndex, rightColumnIndex];
                }
            }
        }

        return result;
    }
}
