using System.Diagnostics;
using MatrixMul.Generators;
using MatrixMul.Matrices;
using MatrixMul.MatrixExceptions;

namespace MatrixMul;

public static class MatrixMain
{
    public static void Main()
    {
        Console.WriteLine("Welcome to high performance matrix multiplication...");
        Console.Write("Please enter the path to the left matrix:");
        Console.Write("Please enter the path to the right matrix:");

        var left2DArray = MatrixGenerator.Generate2DIntArray(1000, 1000);
        var right2DArray = MatrixGenerator.Generate2DIntArray(1000, 1000);

        /*var leftSeqIntMatrix = new IntSequentialMatrix(left2DArray);
        var rightSeqIntMatrix = new IntSequentialMatrix(right2DArray);

        var leftParIntMatrix = new IntParallelMatrix(left2DArray);
        var rightParIntMatrix = new IntParallelMatrix(right2DArray);*/
        
        var stopwatch = new Stopwatch();
        
        stopwatch.Start();
        var result = MatrixOperations.Int2DArraySequentialMul(left2DArray, right2DArray);
        stopwatch.Stop();

        Console.Write($"Seq: {stopwatch.ElapsedMilliseconds}");
    }

    private static long GetComputationTime(IntMatrix leftMatrix, IntMatrix rightMatrix)
    {
        var stopwatch = new Stopwatch();
        
        stopwatch.Start();
        var resultMatrix = PolymorphicIntMatrixMul(leftMatrix, rightMatrix);
        stopwatch.Stop();
        
        
        
        return stopwatch.ElapsedMilliseconds;
    }

    private static IntMatrix PolymorphicIntMatrixMul(IntMatrix left, IntMatrix right)
        => (left, right) switch
        {
            (IntParallelMatrix leftMatrix, IntParallelMatrix rightMatrix) => leftMatrix * rightMatrix,
            (IntSequentialMatrix leftMatrix, IntSequentialMatrix rightMatrix) => leftMatrix * rightMatrix,
            _ => throw new IntMatrixMulException("TODO1")
        };
}
