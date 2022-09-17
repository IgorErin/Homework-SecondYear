using System.Diagnostics;
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
        var leftRowCount = ReadInputCountWithMessage("Enter the number of rows in the left matrix: ");
        var leftColumnCount = ReadInputCountWithMessage("Enter the number of columns in the left matrix: ");
        
        var rightRowCount = ReadInputCountWithMessage("Enter the number of rows in the right matrix: ");
        var rightColumnCount = ReadInputCountWithMessage("Enter the number of columns in the right matrix: ");

        
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
        var localCount = 0;
        
        Console.Write(message);
        
        while (!int.TryParse(Console.ReadLine(), out localCount))
        {
            Console.Write(message);
        }

        return localCount;
    }
}
