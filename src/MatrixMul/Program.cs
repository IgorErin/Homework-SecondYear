using System.Diagnostics;
using MatrixMul.Generators;
using MatrixMul.MatrixExceptions;
using MatrixMul.Readers;
using MatrixMul.Writers;

namespace MatrixMul;

public static class MatrixMain
{
    private const string DefaultOutputPath = ""; //TODO()
    public static void Main()
    {
        Console.WriteLine("Welcome to high performance matrix multiplication...");
        Console.WriteLine("You want to multiply your matrices, otherwise a test measurement will be performed? [yes/no]");
        
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
            TestMul();
        }
    }

    private static bool IsCorrectAnswer(string? answer, params string[] correctAnswers)
        => Array.Exists(correctAnswers, s => s == answer);
    
    private static void TestMul()
    {
        var runCount = ReadInputCountWithMessage("Please enter the number of test runs");
        
        var parTimeArray = new double[runCount];
        var seqTimeArray = new double[runCount];

        var parMulFun = MatrixOperations.Int2DArrayParallelMul;
        var seqMulFun = MatrixOperations.Int2DArraySequentialMul;


        for (var elementPower = 0; elementPower < 10; elementPower++)
        {
            for (var runIndex = 0; runIndex < runCount; runIndex++)
            {
                Console.WriteLine("gen arrays...");
                
                var elementCount = 200 * (2 ** elementPower)

                var leftInt2DArray = ArrayGenerator.Generate2DIntArray(100 + 2 * elementPower, elementCount);
                var rightInt2DArray = ArrayGenerator.Generate2DIntArray(elementCount, elementCount);

                Console.WriteLine($"Execute parallel multiplication #{runIndex}");

                var currentParTimeResult = GetExecutionTime(parMulFun, leftInt2DArray, rightInt2DArray);

                Console.WriteLine($"Execute sequential multiplication #{runIndex}");

                var currentSeqTimeResult = GetExecutionTime(seqMulFun, leftInt2DArray, rightInt2DArray);

                parTimeArray[runIndex] = currentParTimeResult;
                seqTimeArray[runIndex] = currentSeqTimeResult;
            }
            
            var resultParAverageTime = Enumerable.Average(parTimeArray);
            var resultSeqAverageTime = Enumerable.Average(seqTimeArray);
        }
        
        
    }

    private static void UserMatrixMul()
    {
        Console.Write("Please enter the path to the left matrix: ");
        
        var leftMatrixPath = Console.ReadLine();
        while (Path.GetFileName(leftMatrixPath) == null)
        {
            Console.Write("Incorrect path, try again");
        
            leftMatrixPath = Console.ReadLine();
        }
        
        Console.Write("Please enter the path to the right matrix: ");
        
        var rightMatrixPath = Console.ReadLine();
        while (Path.GetFileName(rightMatrixPath) == null)
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
        var stopwatch = new Stopwatch();
        
        stopwatch.Start();
        var result = mulFunc(left2DArray, right2DArray);
        stopwatch.Stop();

        return stopwatch.ElapsedMilliseconds;
    }
    
    
}
