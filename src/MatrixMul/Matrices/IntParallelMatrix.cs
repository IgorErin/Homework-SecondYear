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
        var rightColumnCount = rightArray.GetLength(1);

        var taskArray = new Thread[ThreadCount];
        var currentThreadNum = 0;

        var result = new int[leftRowCount, rightColumnCount];

        for (var resultRowIndex = 0; resultRowIndex < leftRowCount; resultRowIndex++)
        {
            for (var resultColumnIndex = 0; resultColumnIndex < rightColumnCount; resultColumnIndex++)
            {
                if (currentThreadNum >= ThreadCount)
                {
                    ExecuteArray(taskArray, currentThreadNum);
                    currentThreadNum = 0;
                }
                
                var localColumnIndex = resultColumnIndex;
                var localRowIndex = resultRowIndex;
                
                var thread = new Thread(() =>
                    {
                        var resultItem = GetResultItem(localColumnIndex, localRowIndex, leftArray, rightArray);

                        result[localRowIndex, localColumnIndex] = resultItem;
                    }
                );

                taskArray[currentThreadNum] = thread;
                currentThreadNum++;
            }
        }
        
        ExecuteArray(taskArray, currentThreadNum);

        return result;
    }

    private static void ExecuteArray(Thread[] threadArray, int threadNum)
    {
        for (var threadIndex = 0; threadIndex < threadNum; threadIndex++)
        {
            threadArray[threadIndex].Start();
        }

        for (var threadIndex = 0; threadIndex < threadNum; threadIndex++)
        {
            threadArray[threadIndex].Join();
        }
    } 
}