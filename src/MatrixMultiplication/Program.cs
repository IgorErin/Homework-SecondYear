namespace MatrixMultiplication;

using Strategies;
using System.Diagnostics;
using Extensions;
using Generators;
using Readers;
using Writers;

/// <summary>
/// Main class.
/// </summary>
public static class MatrixMain
{
    /// <summary>
    /// Values for table builder. Used in <see cref="ConvertArraysToString"/> method.
    /// </summary>
    private const int MaxLength = 15;
    private const int MessageDelta = 25;

    /// <summary>
    /// Message for table builder. Used in <see cref="ConvertArraysToString"/> method.
    /// </summary>
    private const string ElementCountRowMessage = "element count";
    private const string AverSeqTimeRowMessage = "average sequential time";
    private const string AverParTimeRowMessage = "average parallel time";
    private const string DevSeqTimeRowMessage = "standard deviation sequential time";
    private const string DevParTimeRowMessage = "standard deviation parallel time";

    /// <summary>
    /// Start and delta values for test run. Used in <see cref="ExecuteTestMultiplications"/> method.
    /// </summary>
    private const int StartTestElementsCount = 100;

    /// <summary>
    /// Default test run count. Used in <see cref="ExecuteTestMultiplications"/> method.
    /// </summary>
    private const int DefaultRunWithFixedSizeCount = 5;

    private const int TestRunArgsLength = 2;
    private const int UserRunArgsLength = 5;

    private const int RoundNumber = 3;

    /// <summary>
    /// Main function of a program.
    /// </summary>
    /// <param name="args">CLI args.</param>
    public static void Main(string[] args)
    {
        try
        {
            ParsArgs(args);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);

            Console.WriteLine("In the case of a test run, you must specify the 'test' and the number of test runs.");
            Console.WriteLine("In the case of calculating your matrix, you must specify the 'user',");
            Console.WriteLine("then the path to the left matrix, to the right,");
            Console.WriteLine("the method of multiplying them [sequential/parallel] and the resulting path.");
        }
        finally
        {
            Console.WriteLine("Please, restart the program");
        }
    }

    private static Unit ParsArgs(string[] args) =>
        args.Length switch
        {
            TestRunArgsLength => TestRun(args),
            UserRunArgsLength => UserRun(args),
            _ => throw new ArgumentException("Incorrect number of arguments")
        };

    private static Unit TestRun(string[] args)
    {
        if (args[0] != "test")
        {
            throw new ArgumentException("Expected 'test' in first argument");
        }

        if (int.TryParse(args[1], out var count))
        {
            ExecuteTestMultiplications(count);
        }
        else
        {
            throw new ArgumentException("expected int value in second argument");
        }

        return new Unit();
    }

    private static Unit UserRun(string[] args)
    {
        var mode = args[0];
        var pathToLeftMatrix = args[1];
        var pathToRightMatrix = args[2];
        var stringStrategy = args[3];
        var outputPath = args[4];

        if (mode != "user")
        {
            throw new ArgumentException("expected 'user' in first argument");
        }

        var leftMatrix = GetMatrixFromPath(pathToLeftMatrix);
        var rightMatrix = GetMatrixFromPath(pathToRightMatrix);

        IMultiplicationStrategy strategy =
            stringStrategy switch
            {
                "sequential" => new SequentialStrategy(),
                "parallel" => new ParallelStrategy(),
                _ => throw new ArgumentException("multiplication strategy not match (sequential/parallel)")
            };

        ExecuteUserMultiplications(leftMatrix, rightMatrix, strategy, outputPath);

        return new Unit();
    }

    /// <summary>
    /// Test run of multiplications with calculation of expectation and standard deviation
    /// And printing of a table with results.
    /// </summary>
    private static void ExecuteTestMultiplications(int runCount)
    {
        var parResultTime = new double[DefaultRunWithFixedSizeCount];
        var seqResultTime = new double[DefaultRunWithFixedSizeCount];

        var parResultDeviation = new double[DefaultRunWithFixedSizeCount];
        var seqResultDeviation = new double[DefaultRunWithFixedSizeCount];

        var resultElementCounts = new int[DefaultRunWithFixedSizeCount];

        var stopWatch = new Stopwatch();

        for (var globalRunIndex = 0; globalRunIndex < DefaultRunWithFixedSizeCount; globalRunIndex++)
        {
            var parTimeArray = new double[runCount];
            var seqTimeArray = new double[runCount];

            var elementCount = StartTestElementsCount * 2.IntPow(globalRunIndex);

            var parallelStrategy = new ParallelStrategy();
            var sequentialStrategy = new SequentialStrategy();

            for (var runIndex = 0; runIndex < runCount; runIndex++)
            {
                Console.WriteLine("Gen arrays...");

                var leftInt2DArray = IntArrayGenerator.Generate2DIntArray(elementCount, elementCount);
                var rightInt2DArray = IntArrayGenerator.Generate2DIntArray(elementCount, elementCount);

                var leftMatrix = new IntMatrix(leftInt2DArray);
                var rightMatrix = new IntMatrix(rightInt2DArray);

                Console.WriteLine($"Executing multiplication #{runIndex} with {elementCount} elements");

                var currentParTimeResult =
                    stopWatch.ResetAndGetTimeOfIntMatrixMultiplication(parallelStrategy, leftMatrix, rightMatrix);

                var currentSeqTimeResult =
                    stopWatch.ResetAndGetTimeOfIntMatrixMultiplication(sequentialStrategy, leftMatrix, rightMatrix);

                parTimeArray[runIndex] = Math.Round(currentParTimeResult, RoundNumber);
                seqTimeArray[runIndex] = Math.Round(currentSeqTimeResult, RoundNumber);
            }

            parResultTime[globalRunIndex] = Enumerable.Average(parTimeArray);
            seqResultTime[globalRunIndex] = Enumerable.Average(seqTimeArray);

            parResultDeviation[globalRunIndex] = Math.Round(parTimeArray.GetDeviation(), RoundNumber);
            seqResultDeviation[globalRunIndex] = Math.Round(seqTimeArray.GetDeviation(), RoundNumber);

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
    private static void ExecuteUserMultiplications(
        IntMatrix leftMatrix,
        IntMatrix rightMatrix,
        IMultiplicationStrategy strategy,
        string resultPath)
    {
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        var result = leftMatrix.MultiplyWithStrategy(rightMatrix, strategy);
        stopwatch.Stop();

        Int2DArrayToTextFileWriter.Write(resultPath, result.ToArray);

        Console.WriteLine($"Multiplication done in {stopwatch.ElapsedMilliseconds} milliseconds");
        Console.WriteLine($"the result is written in {resultPath}");
    }

    /// <summary>
    /// a method that tries to read the path from the console, displaying messages.
    /// </summary>
    /// <param name="path">message printed on first try.</param>
    /// <returns>file path as a string.</returns>
    /// <exception cref="IOException">An exception will be thrown if the arrays have different lengths.</exception>
    private static IntMatrix GetMatrixFromPath(string path)
    {
        if (File.Exists(path))
        {
            var array = TextFileToInt2DArrayReader.Read(path);

            return new IntMatrix(array);
        }

        throw new ArgumentException($"file does not exist in {path} ");
    }

    /// <summary>
    /// A function that builds a table with values passed as parameters.
    /// With the same length.
    /// </summary>
    /// <param name="itemCount">An array of int values located in the first row of the table.</param>
    /// <param name="seqDeviation">An array of double values located in the second row of the table.</param>
    /// <param name="parDeviation">An array of double values located in the third row of the table.</param>
    /// <param name="seqTimes">An array of double values located in the four row of the table.</param>
    /// <param name="parTimes">An array of double values located in the five row of the table.</param>
    /// <returns>String representation of a table.</returns>
    /// <exception cref="ArgumentException">An exception will be thrown if the arrays have different lengths.</exception>
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
        tableWriter.WriteRow(itemCount, ElementCountRowMessage, MessageDelta);

        tableWriter.WriteBound(length, MessageDelta);
        tableWriter.WriteRow(seqTimes, AverSeqTimeRowMessage, MessageDelta);

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
