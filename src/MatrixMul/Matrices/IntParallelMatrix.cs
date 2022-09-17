using MatrixMul.MatrixExceptions;

namespace MatrixMul.Matrices;

public class IntParallelMatrix : IntMatrix
{
    private const int ThreadCount = 8; 
    public IntParallelMatrix(int[,] intArray) : base(intArray)
    {
    }

    public static IntParallelMatrix operator *(IntParallelMatrix leftMatrix, IntParallelMatrix rightMatrix)
    {
        if (NotAvailableForMultiplication(leftMatrix, rightMatrix))
        {
            throw new IntMatrixMulException("matrix multiplication is not possible, wrong dimension");
        }

        var result = Multiply(leftMatrix.IntArray, rightMatrix.IntArray);
        
        return new IntParallelMatrix(result);
    }

    private static int[,] Multiply(int[,] leftArray, int[,] rightArray)
    {
        var leftRowCount = leftArray.GetLength(0);
        var leftColumnCount = leftArray.GetLength(1);

        var rightRowCount = rightArray.GetLength(0);
        var rightColumnCount = rightArray.GetLength(1);

        if (leftColumnCount != rightRowCount)
        {
            throw new MatrixOperationsException(
                $"matrix multiplication is not possible, wrong dimension: {leftColumnCount} != {rightRowCount}"
                );
        }

        var forLoopRowBounds = new int[ThreadCount + 1];
        forLoopRowBounds[ThreadCount] = leftRowCount;
        
        var lenPiece = (int) Math.Ceiling(leftRowCount / (double) ThreadCount);

        for (var threadIndex = 0; threadIndex < ThreadCount; threadIndex++)
        {
            forLoopRowBounds[threadIndex] = threadIndex * lenPiece;
        }
        
        var result = new int[leftRowCount, rightColumnCount];
        var threads = new Thread[ThreadCount];

        for (var threadIndex = 0; threadIndex < ThreadCount; threadIndex++)
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
                                    leftArray[leftRowIndex, currentItemIndex] *
                                    rightArray[currentItemIndex, rightColumnIndex];
                            }
                        }
                    }
                }
            );

            threads[threadIndex] = currentThread;
        }

        ExecuteArray(threads);

        return result;
    }

    private static void ExecuteArray(Thread[] threadArray)
    {
        foreach (var thread in threadArray)
        {
            thread.Start();
        }

        foreach (var thread in threadArray)
        {
            thread.Join();
        }
    } 
}