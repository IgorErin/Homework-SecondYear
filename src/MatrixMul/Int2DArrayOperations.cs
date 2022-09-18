using MatrixMul.MatrixExceptions;

namespace MatrixMul;

/// <summary>
/// Static class providing int two-dimensional array multiplication operations.
/// </summary>
public static class Int2DArrayOperations
{
    /// <summary>
    /// A function that performs parallel multiplication of two two-dimensional arrays.
    /// </summary>
    /// <param name="leftMatrix">Left array passed for multiplication</param>
    /// <param name="rightMatrix">Right array passed for multiplication</param>
    /// <returns>Result of multiplication</returns>
    /// <exception cref="Int2DArrayOperationsException">Exception was thrown if the arrays have wrong dimension</exception>
    public static int[,] Int2DArrayParallelMul(int[,] leftMatrix, int[,] rightMatrix)
    {
        var threadCount = Environment.ProcessorCount;
        
        var leftRowCount = leftMatrix.GetLength(0);
        var leftColumnCount = leftMatrix.GetLength(1);

        var rightRowCount = rightMatrix.GetLength(0);
        var rightColumnCount = rightMatrix.GetLength(1);

        if (leftColumnCount != rightRowCount)
        {
            throw new Int2DArrayOperationsException(
                $"matrix multiplication is not possible, wrong dimension: {leftColumnCount} != {rightRowCount}"
                );
        }

        var forLoopRowBounds = new int[threadCount + 1];
        forLoopRowBounds[threadCount] = leftRowCount;
        
        var lenPiece = (int) Math.Ceiling(leftRowCount / (double) threadCount) - 1;

        for (var threadIndex = 0; threadIndex < threadCount; threadIndex++)
        {
            forLoopRowBounds[threadIndex] = threadIndex * lenPiece;
        }

        var result = new int[leftRowCount, rightColumnCount];
        var threads = new Thread[threadCount];

        for (var threadIndex = 0; threadIndex < threadCount; threadIndex++)
        {
            var currentLowBound = forLoopRowBounds[threadIndex];
            var currentHighBound = forLoopRowBounds[threadIndex + 1];
            
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
                }
            );

            threads[threadIndex] = currentThread;
        }

        StartAndJoinThreadArray(threads);

        return result;
    }

    /// <summary>
    /// Start and join each thread in the array.
    /// </summary>
    /// <param name="threads">Array of Thread.</param>
    private static void StartAndJoinThreadArray(Thread[] threads)
    {
        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
    
    /// <summary>
    /// A function that performs sequential multiplication of two two-dimensional arrays.
    /// </summary>
    /// <param name="leftMatrix">Left array passed for multiplication</param>
    /// <param name="rightMatrix">Right array passed for multiplication</param>
    /// <returns>Result of multiplication</returns>
    /// <exception cref="Int2DArrayOperationsException">Exception was thrown if the arrays have wrong dimension</exception>
    public static int[,] Int2DArraySequentialMul(int[,] leftMatrix, int[,] rightMatrix)
    {
        var leftRowCount = leftMatrix.GetLength(0);
        var leftColumnCount = leftMatrix.GetLength(1);

        var rightRowCount = rightMatrix.GetLength(0);
        var rightColumnCount = rightMatrix.GetLength(1);

        if (leftColumnCount != rightRowCount)
        {
            throw new Int2DArrayOperationsException(
                $"matrix multiplication is not possible, wrong dimension: {leftColumnCount} != {rightRowCount}"
            );
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
