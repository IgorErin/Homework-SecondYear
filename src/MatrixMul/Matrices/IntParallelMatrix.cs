using System.Collections.Generic;
using System.Threading;
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
        if (AvailableForMultiplication(leftMatrix, rightMatrix))
        {
            throw new MatrixMulException("TODO");
        }

        int[, ] result = Multiply(leftMatrix.IntArray, rightMatrix.IntArray);
        
        return new IntParallelMatrix(result);
    }
    
    private static int[,] Multiply(int[,] leftArray, int[,] rightArray)
    {
        var leftRowCount = leftArray.GetLength(0);
        var rightColumnCount = rightArray.GetLength(1);

        var taskList = new List<Thread>();

        int[,] result = new int[leftRowCount, rightColumnCount];

        for (var resultRowIndex = 0; resultRowIndex < leftRowCount; resultRowIndex++)
        {
            for (var resultColumnIndex = 0; resultColumnIndex < rightColumnCount; resultColumnIndex++)
            {
                if (taskList.Count >= ThreadCount)
                {
                    ExecuteList(taskList);
                
                    taskList.Clear();
                }
                
                var localColumnIndex = resultColumnIndex;
                var localRowIndex = resultRowIndex;
                
                var thread = new Thread(() =>
                    {
                        var resultItem = GetResultItem(localColumnIndex, localRowIndex, leftArray, rightArray);

                        result[localRowIndex, localColumnIndex] = resultItem;
                    }
                );

                taskList.Add(thread);
            }
        }
        
        ExecuteList(taskList);

        return result;
    }

    private static void ExecuteList(List<Thread> threadList)
    {
        foreach (var thread in threadList)
        {
            thread.Start();
        }

        foreach (var thread in threadList)
        {
            thread.Join();
        }
    } 
}