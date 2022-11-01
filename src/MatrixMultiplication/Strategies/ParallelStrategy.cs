namespace MatrixMultiplication.Strategies;

using MatrixExceptions;

/// <summary>
/// Class that implements the parallel multiplication strategy.
/// </summary>
public class ParallelStrategy : IMultiplicationStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParallelStrategy"/> class.
    /// </summary>
    public ParallelStrategy()
    {
    }
    
    /// <summary>
    /// Parallel multiplication method.
    /// </summary>
    /// <param name="leftMatrix">left matrix array representation.</param>
    /// <param name="rightMatrix">right matrix array representation.</param>
    /// <returns>Result of multiplication.</returns>
    /// <exception cref="IntMatrixMultiplicationException">
    /// Was thrown when dimensions did not match for multiplication.
    /// </exception>
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
        
        var threadCount = Math.Min(Environment.ProcessorCount, leftRowCount);

        var lengthPiece = (int)Math.Ceiling(leftRowCount / (double)threadCount) - 1;

        var result = new int[leftRowCount, rightColumnCount];
        var countDown = new CountdownEvent(threadCount);

        for (var threadIndex = 0; threadIndex < threadCount; threadIndex++)
        {
            var currentLowBound = threadIndex * lengthPiece;
            var currentHighBound = (threadIndex + 1) * lengthPiece;

            if (threadIndex == threadCount - 1)
            {
                currentHighBound = leftRowCount;
            }

            var currentThread = new Thread(() =>
                {
                    for (var leftRowIndex = currentLowBound; leftRowIndex < currentHighBound; leftRowIndex++)
                    {
                        for (var rightColumnIndex = 0; rightColumnIndex < rightColumnCount; rightColumnIndex++)
                        {
                            for (var currentItemIndex = 0; currentItemIndex < leftColumnCount; currentItemIndex++)
                            {
                                result[leftRowIndex, rightColumnIndex] +=
                                    leftMatrix[leftRowIndex, currentItemIndex] *
                                    rightMatrix[currentItemIndex, rightColumnIndex];
                            }
                        }
                    }

                    countDown.Signal();
                });
            
            currentThread.Start();
        }

        countDown.Wait();

        return result;
    }
}
