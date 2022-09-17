using System.Diagnostics;
using System.Text;
using MatrixMul.Extensions;
using MatrixMul.Generators;
using MatrixMul.Readers;
using MatrixMul.Writers;

namespace MatrixMul;

public static class MatrixMain
{
    private const string DefaultOutputPath = "./resultMatrix"; //TODO()
    private const int DefaultRunWithFixedSizeCount = 10;
    
    private const int MaxLength = 15;
    private const int MessageDelta = 25;

    private const string ElementCountRowMessage = "average element count";
    private const string AverSeqTimeRowMessage = "average sequential time";
    private const string AverParTimeRowMessage = "average parallel time";
    private const string DevSeqTimeRowMessage = "standard deviation sequential time";
    private const string DevParTimeRowMessage = "standard deviation parallel time";

    private const int StartTestElementsCount = 200;
    private const int PoweredDelta = 2;

    public static void Main()
    {
        var left = ArrayGenerator.Generate2DIntArray(100, 100);
        var right = ArrayGenerator.Generate2DIntArray(100, 100);
        
        Int2DArrayToTextFileWriter.WriteToFile("./leftInt2DArray", left);
        Int2DArrayToTextFileWriter.WriteToFile("./rightInt2DArray", right);
        
        
        Console.WriteLine("Welcome to high performance matrix multiplication...");
        Console.WriteLine(
            "You want to multiply your matrices, otherwise a test measurement will be performed? [yes/no]");

        var testRunOrCustomMulAnswer = Console.ReadLine();
        var correctInput = IsCorrectAnswer(testRunOrCustomMulAnswer, "no", "yes");

        while (!correctInput)
        {
            Console.WriteLine("Incorrect answer, try again [yes/no]");

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
        }
    }

    private static bool IsCorrectAnswer(string? answer, params string[] correctAnswers)
        => Array.Exists(correctAnswers, s => s == answer);

    private static void TestMatrixMul()
    {
        var runCount = ReadInputCountWithMessage("Please enter the number of test runs with fixed matrix size: ");

        var parMulFun = MatrixOperations.Int2DArrayParallelMul;
        var seqMulFun = MatrixOperations.Int2DArraySequentialMul;

        var parResultTime = new double[DefaultRunWithFixedSizeCount];
        var seqResultTime = new double[DefaultRunWithFixedSizeCount];
        
        var parResultDeviation = new double[DefaultRunWithFixedSizeCount];
        var seqResultDeviation = new double[DefaultRunWithFixedSizeCount];
        
        var resultElementCounts = new int[DefaultRunWithFixedSizeCount];

        for (var globalRunIndex = 0; globalRunIndex < DefaultRunWithFixedSizeCount; globalRunIndex++)
        {
            var parTimeArray = new double[runCount];
            var seqTimeArray = new double[runCount];

            var elementCount = StartTestElementsCount + PoweredDelta.IntPow(globalRunIndex);

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
            
            parResultDeviation[globalRunIndex] = GetDeviation(parTimeArray);
            seqResultDeviation[globalRunIndex] = GetDeviation(seqTimeArray);
            
            resultElementCounts[globalRunIndex] = elementCount;
        }
        
        Console.WriteLine(ConvertArraysToString(
            parResultTime,
            seqResultTime,
            parResultDeviation,
            seqResultDeviation,
            resultElementCounts));
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

        var rightMatrixPath = Console.ReadLine();
        while (File.Exists(rightMatrixPath))
        {
            Console.Write("Incorrect path, try again");

            leftMatrixPath = Console.ReadLine();
        }

        var left2DArray = TextFileToInt2DArrayReader.GetMatrix(leftMatrixPath); //TODO null ref
        var right2DArray = TextFileToInt2DArrayReader.GetMatrix(rightMatrixPath); //TODO

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

    private static double GetDeviation(double[] values)
        => Enumerable.Average(values.Select(x => x * x)) -
           Enumerable.Average(values) * Enumerable.Average(values);

    private static Func<int[,], int[,], int[,]> GetMulFun(string parallelRunOrSequentialMulAnswer)
        => parallelRunOrSequentialMulAnswer switch
        {
            "par" => MatrixOperations.Int2DArrayParallelMul,
            _ => MatrixOperations.Int2DArraySequentialMul,
        };

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

    private static string ConvertArraysToString(
        double[] parTimes,
        double[] seqTimes, 
        double[] parDeviation,
        double[] seqDeviation,
        int[] itemCount)
    {
        var firstCond = parTimes.Length != seqTimes.Length;
        var secondCond = parTimes.Length != itemCount.Length;
        var thirdCond = seqTimes.Length != itemCount.Length;

        if (firstCond || secondCond || thirdCond)
        {
            throw new ArgumentException("TODO()");
        }

        var length = parTimes.Length + 1;

        var stringBuilder = new StringBuilder();
        
        stringBuilder.Append(ResultTableWriter.WriteBound(length, MaxLength, MessageDelta));
        stringBuilder.Append(ResultTableWriter.WriteRow(itemCount, MaxLength, ElementCountRowMessage, MessageDelta));
        
        stringBuilder.Append(ResultTableWriter.WriteBound(length, MaxLength, MessageDelta));
        stringBuilder.Append(ResultTableWriter.WriteRow(seqTimes, MaxLength, AverSeqTimeRowMessage, MessageDelta));

        stringBuilder.Append(ResultTableWriter.WriteBound(length, MaxLength, MessageDelta));
        stringBuilder.Append(ResultTableWriter.WriteRow(parTimes, MaxLength, AverParTimeRowMessage, MessageDelta));
        
        stringBuilder.Append(ResultTableWriter.WriteBound(length, MaxLength, MessageDelta));
        stringBuilder.Append(ResultTableWriter.WriteRow(parDeviation, MaxLength, DevSeqTimeRowMessage, MessageDelta));
        
        stringBuilder.Append(ResultTableWriter.WriteBound(length, MaxLength, MessageDelta));
        stringBuilder.Append(ResultTableWriter.WriteRow(seqDeviation, MaxLength, DevParTimeRowMessage, MessageDelta));

        stringBuilder.Append(ResultTableWriter.WriteBound(length, MaxLength, MessageDelta));

        return stringBuilder.ToString();
    }
}
