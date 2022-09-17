using System.Data;
using System.Diagnostics;
using System.Text;
using MatrixMul.Generators;
using MatrixMul.Readers;
using MatrixMul.Writers;

namespace MatrixMul;

public static class MatrixMain
{
    private const string DefaultOutputPath = ""; //TODO()
    private const int DefaultRunWithFixedSizeCount = 10;
    private const int ConvertArrayCount = 3;

    public static void Main()
    {
        
        //
        var firstArray = new double[] { 0.4, 0.4, 0.5 };
        var scondArray = new double[] { 0.4, 0.4, 0.5 };
        var thirdArray = new int[] { 0, 4, 0 };
        
        Console.WriteLine(ConvertArraysToTuple(firstArray, scondArray, thirdArray));
        //
        /*Console.WriteLine("Welcome to high performance matrix multiplication...");
        Console.WriteLine(
            "You want to multiply your matrices, otherwise a test measurement will be performed? [yes/no]");

        var testRunOrCustomMulAnswer = Console.ReadLine();
        var correctInput = IsCorrectAnswer(testRunOrCustomMulAnswer, "no", "yes");

        while (!correctInput)
        {
            Console.Write("Incorrect answer, try again");

            testRunOrCustomMulAnswer = Console.ReadLine();
            correctInput = IsCorrectAnswer(testRunOrCustomMulAnswer, "no", "yes");
        }

        if (testRunOrCustomMulAnswer == "yes")
        {
            UserMatrixMul();
        }
        else
        {
            TestMatrixMul();
        }*/
    }

    private static bool IsCorrectAnswer(string? answer, params string[] correctAnswers)
        => Array.Exists(correctAnswers, s => s == answer);

    private static void TestMatrixMul()
    {
        var runCount = ReadInputCountWithMessage("Please enter the number of test runs with fixed matrix size");

        var parMulFun = MatrixOperations.Int2DArrayParallelMul;
        var seqMulFun = MatrixOperations.Int2DArraySequentialMul;

        var parResultTime = new double[DefaultRunWithFixedSizeCount];
        var seqResultTime = new double[DefaultRunWithFixedSizeCount];
        var resultElementCounts = new int[DefaultRunWithFixedSizeCount];

        for (var globalRunIndex = 0; globalRunIndex < DefaultRunWithFixedSizeCount; globalRunIndex++)
        {
            var parTimeArray = new double[runCount];
            var seqTimeArray = new double[runCount];

            var elementCount = 200 + 2.IntPow(globalRunIndex);

            for (var runIndex = 0; runIndex < runCount; runIndex++)
            {
                Console.WriteLine("Gen arrays...");

                var leftInt2DArray = ArrayGenerator.Generate2DIntArray(elementCount, elementCount);
                var rightInt2DArray = ArrayGenerator.Generate2DIntArray(elementCount, elementCount);

                Console.WriteLine($"Executing multiplication #{runIndex} with {elementCount} elements");

                var currentParTimeResult = GetExecutionTime(parMulFun, leftInt2DArray, rightInt2DArray);
                var currentSeqTimeResult = GetExecutionTime(seqMulFun, leftInt2DArray, rightInt2DArray);

                parTimeArray[runIndex] = currentParTimeResult;
                seqTimeArray[runIndex] = currentSeqTimeResult;
            }

            parResultTime[globalRunIndex] = Enumerable.Average(parTimeArray);
            seqResultTime[globalRunIndex] = Enumerable.Average(seqTimeArray);
            resultElementCounts[globalRunIndex] = elementCount;
        }

        //Console.WriteLine(ResultTableWriter.WriteTableToFile((parResultTime, seqResultTime, resultElementCounts))); TODO()
    }

    private static void UserMatrixMul()
    {
        Console.Write("Please enter the path to the left matrix: ");

        var leftMatrixPath = Console.ReadLine();
        while (File.Exists(leftMatrixPath))
        {
            Console.Write("Incorrect path, try again");

            leftMatrixPath = Console.ReadLine();
        }

        Console.Write("Please enter the path to the right matrix: ");

        var rightMatrixPath = Console.ReadLine() ?? "";
        while (File.Exists(rightMatrixPath))
        {
            Console.Write("Incorrect path, try again");

            leftMatrixPath = Console.ReadLine();
        }

        var left2DArray = TextFileToInt2DArrayReader.GetMatrix(leftMatrixPath);
        var right2DArray = TextFileToInt2DArrayReader.GetMatrix(rightMatrixPath);

        Console.WriteLine("Perform parallel multiplication or serial multiplication");
        Console.WriteLine("enter \"par\" for parallel and \"seq\" for sequential respectively");

        var parallelOrSequentialMulAnswer = Console.ReadLine();
        var correctInput = IsCorrectAnswer(parallelOrSequentialMulAnswer, "par", "seq");

        while (!correctInput)
        {
            Console.Write("Incorrect answer, try again");

            parallelOrSequentialMulAnswer = Console.ReadLine();
            correctInput = IsCorrectAnswer(parallelOrSequentialMulAnswer, "par", "seq");
        }

        var mulFun = GetMulFun(parallelOrSequentialMulAnswer ?? "seq");
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        var result = mulFun(left2DArray, right2DArray);
        stopwatch.Stop();

        Int2DArrayToTextFileWriter.WriteToFile(DefaultOutputPath, result);

        Console.WriteLine($"Multiplication done in {stopwatch.ElapsedMilliseconds} milliseconds");
        Console.WriteLine($"the result is written in {DefaultOutputPath}");
    }

    private static Func<int[,], int[,], int[,]> GetMulFun(string parallelRunOrSequentialMulAnswer)
        => parallelRunOrSequentialMulAnswer switch
        {
            "par" => MatrixOperations.Int2DArrayParallelMul,
            _ => MatrixOperations.Int2DArraySequentialMul,
        };

    private static int[,] ExecuteMul(Func<int[,], int[,], int[,]> func, int[,] leftArray, int[,] rightArray)
        => func(leftArray, rightArray);

    private static int ReadInputCountWithMessage(string message)
    {
        int localCount;

        Console.Write(message);

        while (!int.TryParse(Console.ReadLine(), out localCount))
        {
            Console.Write(message);
        }

        return localCount;
    }

    private static long GetExecutionTime(Func<int[,], int[,], int[,]> mulFunc, int[,] left2DArray, int[,] right2DArray)
    {
        var stopwatch = new Stopwatch(); //TODO()

        stopwatch.Start();
        var result = mulFunc(left2DArray, right2DArray);
        stopwatch.Stop();

        return stopwatch.ElapsedMilliseconds;
    }

    private static string ConvertArraysToTuple(double[] parTimes, double[] seqTimes, int[] itemCount) //TODO
    {
        var firstCond = parTimes.Length != seqTimes.Length;
        var secondCond = parTimes.Length != itemCount.Length;
        var thirdCond = seqTimes.Length != itemCount.Length;

        if (firstCond || secondCond || thirdCond)
        {
            throw new ArgumentException("TODO()");
        }

        var length = parTimes.Length;

        var stringBuilder = new StringBuilder();
        
        stringBuilder.AppendFormat("+{0}+", new String('-', 100));

        return stringBuilder.ToString();
    }
}
