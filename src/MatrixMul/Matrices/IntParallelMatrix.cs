using MatrixMul.MatrixExceptions;

namespace MatrixMul.Matrices;

/// <summary>
/// Base class implementation of IntMatrix with parallel multiplication.
/// </summary>
public class IntParallelMatrix : IntMatrix
{
    /// <summary>
    /// Constructor initializing the base class.
    /// </summary>
    /// <param name="intArray">Base class constructor parameter, int 2D array matrix representation</param>
    public IntParallelMatrix(int[,] intArray) : base(intArray)
    {
    }

    /// <summary>
    /// Method of parallel multiplication of two IntParallel matrices. 
    /// </summary>
    /// <param name="leftMatrix">IntParallel matrix stand on the left when multiplied</param>
    /// <param name="rightMatrix">IntParallel matrix stand on the right when multiplied</param>
    /// <returns>IntParallel matrix - parallel multiplication result</returns>
    /// <exception cref="IntMatrixMulException">
    /// An exception will be thrown when matrices have dimensions that are incorrect for multiplication
    /// </exception>
    public static IntParallelMatrix operator *(IntParallelMatrix leftMatrix, IntParallelMatrix rightMatrix)
    {
        if (!AvailableForMultiplication(leftMatrix, rightMatrix))
        {
            var leftRowLen = leftMatrix.GetLength(1);
            var rightColumnLen = rightMatrix.GetLength(0);
            
            throw new IntMatrixMulException(
                $"matrix multiplication is not possible, wrong dimension: {leftRowLen} != {rightColumnLen}");
        }

        var result = Multiply(leftMatrix.IntArray, rightMatrix.IntArray);
        
        return new IntParallelMatrix(result);
    }

    /// <summary>
    /// Method of parallel multiplication of two int 2D arrays. 
    /// </summary>
    /// <param name="leftArray">Int 2D array stand on the left when multiplied</param>
    /// <param name="rightArray">Int 2D array stand on the right when multiplied</param>
    /// <returns>Result int 2D array</returns>
    private static int[,] Multiply(int[,] leftArray, int[,] rightArray)
    {
        var threadCount = Environment.ProcessorCount;
        
        var leftRowCount = leftArray.GetLength(0);
        var rightColumnCount = rightArray.GetLength(1);

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
                            var resultItem = GetResultItem(leftRowIndex, rightColumnIndex, leftArray, rightArray);
                            
                            result[leftRowIndex, rightColumnIndex] = resultItem;
                        }

                    }
                }
            );

        threads[threadIndex] = currentThread;
        }

        ExecuteArray(threads);

        return result;
    }
    
    /// <summary>
    /// Start and join each thread in the array.
    /// </summary>
    /// <param name="threads">Array of Threads.</param>
    private static void ExecuteArray(Thread[] threads)
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
}