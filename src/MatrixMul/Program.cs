using System.Diagnostics;
using MatrixMul.Extensions;
using MatrixMul.Generators;
using MatrixMul.Readers;
using MatrixMul.Writers;

namespace MatrixMul;

public static class MatrixMain
{
    /// <summary>
    /// Default path to save result of user multiplication in. Used in UserMatrixMul.
    /// </summary>
    private const string DefaultOutputPath = "./resultMatrix";
    
    /// <summary>
    /// Values for table builder. Used in ConvertArraysToString method.
    /// </summary>
    private const int MaxLength = 15;
    private const int MessageDelta = 25;

    /// <summary>
    /// Message for table builder. Used in ConvertArraysToString method.
    /// </summary>
    private const string ElementCountRowMessage = "average element count";
    private const string AverSeqTimeRowMessage = "average sequential time";
    private const string AverParTimeRowMessage = "average parallel time";
    private const string DevSeqTimeRowMessage = "standard deviation sequential time";
    private const string DevParTimeRowMessage = "standard deviation parallel time";
    
    /// <summary>
    /// Start and delta values for test run. Used in TestMatrixMul method.
    /// </summary>
    private const int StartTestElementsCount = 200;
    private const int PoweredDelta = 2;
    
    /// <summary>
    /// Default test run count. Used in TestMatrixMul method.
    /// </summary>
    private const int DefaultRunWithFixedSizeCount = 10;

    /// <summary>
    /// Main function of a program.
    /// </summary>
    public static void Main()
    {
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

        try
        {
            if (testRunOrCustomMulAnswer == "yes")
            {
                UserMatrixMul();
            }
            else
            {
                TestMatrixMul();
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine("Please, restart the program");
        }
    }

    /// <summary>
    /// Allows you to match the entered value with constant strings.
    /// </summary>
    /// <param name="answer">The string value to be found.</param>
    /// <param name="correctAnswers">Strings among which the search will be carried out.</param>
    /// <returns>true if the value is contained in the array otherwise false.</returns>
    private static bool IsCorrectAnswer(string answer, params string[] correctAnswers)
        => Array.Exists(correctAnswers, s => s == answer);

    /// <summary>
    /// Test run of multiplications with calculation of expectation and standard deviation
    /// And printing of a table with results.
    /// </summary>
    private static void TestMatrixMul()
    {
        var runCount = ReadInputCountWithMessage("Please enter the number of test runs with fixed matrix size: ");

        var parMulFun = Int2DArrayOperations.Int2DArrayParallelMul;
        var seqMulFun = Int2DArrayOperations.Int2DArraySequentialMul;

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

                var leftInt2DArray = IntArrayGenerator.Generate2DIntArray(elementCount, elementCount);
                var rightInt2DArray = IntArrayGenerator.Generate2DIntArray(elementCount, elementCount);

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
            resultElementCounts,
            seqResultDeviation,
            parResultDeviation,
            seqResultTime,
            parResultTime));
    }
    
    /// <summary>
    /// Script with reading two matrices from files by multiplying them and saving the result to a file.
    /// </summary>
    private static void UserMatrixMul()
    {
        Console.Write("Please enter the path to the left matrix: ");

        var leftMatrixPath = Console.ReadLine();
        while (!File.Exists(leftMatrixPath))
        {
            Console.Write("Incorrect path, try again");

            leftMatrixPath = Console.ReadLine();
        }

        Console.Write("Please enter the path to the right matrix: ");

        var rightMatrixPath = Console.ReadLine();
        while (!File.Exists(rightMatrixPath))
        {
            Console.Write("Incorrect path, try again");

            leftMatrixPath = Console.ReadLine();
        }

        var left2DArray = TextFileToInt2DArrayReader.Read(leftMatrixPath);
        var right2DArray = TextFileToInt2DArrayReader.Read(rightMatrixPath); 

        Console.WriteLine("Perform parallel multiplication or sequential multiplication");
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

        Int2DArrayToTextFileWriter.Write(DefaultOutputPath, result);

        Console.WriteLine($"Multiplication done in {stopwatch.ElapsedMilliseconds} milliseconds");
        Console.WriteLine($"the result is written in {DefaultOutputPath}");
    }
    
    /// <summary>
    /// A function that finds the standard deviation of a value from a given array.
    /// </summary>
    /// <param name="values">An array of doubles among which the standard deviation will be calculated</param>
    /// <returns>Standard deviation, double value</returns>
    private static double GetDeviation(double[] values)
        => Enumerable.Average(values.Select(x => x * x)) -
           Enumerable.Average(values) * Enumerable.Average(values);

    /// <summary>
    /// A function that returns one possible multiplication as a function based on a string value.
    /// If the parameter is equal to "par", the function will return a parallel multiplication, otherwise - sequential.
    /// </summary>
    /// <param name="parallelRunOrSequentialMulAnswer">name of multiplication function way: "par" or "seq".</param>
    /// <returns>Multiplication function.</returns>
    private static Func<int[,], int[,], int[,]> GetMulFun(string parallelRunOrSequentialMulAnswer)
        => parallelRunOrSequentialMulAnswer switch
        {
            "par" => Int2DArrayOperations.Int2DArrayParallelMul,
            _ => Int2DArrayOperations.Int2DArraySequentialMul,
        };
    
    /// <summary>
    /// Print parameter message and try to read input int value in while loop.
    /// If there are too many attempts, an exception will be thrown.
    /// </summary>
    /// <param name="message">The message that will be printed as a string.</param>
    /// <returns>The int value of what was read.</returns>
    /// <exception cref="IOException">Exception thrown on high number of failed attempts.</exception>
    private static int ReadInputCountWithMessage(string message)
    {
        int localCount;
        var readCount = 0;

        Console.Write(message);

        while (!int.TryParse(Console.ReadLine(), out localCount))
        {
            Console.Write("incorrect value. " + message);
            readCount++;
            
            if (readCount >= 10)
            {
                throw new IOException("Too many attempts to enter a value, please try again later");
            }
        }

        return localCount;
    }

    /// <summary>
    /// Exception thrown on high number of failed attempts.
    /// </summary>
    /// <param name="mulFunc">Function whose execution time will be calculated.</param>
    /// <param name="left2DArray">Array passed as left argument to function.</param>
    /// <param name="right2DArray">Array passed as right argument to function.</param>
    /// <returns>The result of the passed function applied to the two passed arrays.</returns>
    private static long GetExecutionTime(Func<int[,], int[,], int[,]> mulFunc, int[,] left2DArray, int[,] right2DArray)
    {
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        mulFunc(left2DArray, right2DArray);
        stopwatch.Stop();

        return stopwatch.ElapsedMilliseconds;
    }
    
    /// <summary>
    /// A function that builds a table with values passed as parameters.
    /// With the same length.
    /// </summary>
    /// <param name="itemCount">An array of int values located in the first row of the table</param>
    /// <param name="seqDeviation">An array of double values located in the second row of the table</param>
    /// <param name="parDeviation">An array of double values located in the third row of the table</param>
    /// <param name="seqTimes">An array of double values located in the four row of the table</param>
    /// <param name="parTimes">An array of double values located in the five row of the table</param>
    /// <returns>String representation of a table</returns>
    /// <exception cref="ArgumentException">An exception will be thrown if the arrays have different lengths</exception>
    private static string ConvertArraysToString(
        int[] itemCount,
        double[] seqDeviation,
        double[] parDeviation,
        double[] seqTimes,
        double[] parTimes)
    {
        var firstCond = parTimes.Length != seqTimes.Length;
        var secondCond = parTimes.Length != itemCount.Length;
        var thirdCond = seqTimes.Length != itemCount.Length;

        if (firstCond || secondCond || thirdCond)
        {
            throw new ArgumentException("Array sizes are different");
        }

        var length = parTimes.Length + 1;

        var tableWriter = new StringTableBuilder(MaxLength);
        
        tableWriter.WriteBound(length, MessageDelta);
        tableWriter.WriteRow<int>(itemCount, ElementCountRowMessage, MessageDelta);
        
        tableWriter.WriteBound(length, MessageDelta);
        tableWriter.WriteRow<double>(seqTimes, AverSeqTimeRowMessage, MessageDelta);

        tableWriter.WriteBound(length, MessageDelta);
        tableWriter.WriteRow(parTimes, AverParTimeRowMessage, MessageDelta);
        
        tableWriter.WriteBound(length, MessageDelta);
        tableWriter.WriteRow(parDeviation, DevSeqTimeRowMessage, MessageDelta);
        
        tableWriter.WriteBound(length, MessageDelta);
        tableWriter.WriteRow(seqDeviation, DevParTimeRowMessage, MessageDelta);

        tableWriter.WriteBound(length, MessageDelta);

        return tableWriter.ToString();
    }
}
