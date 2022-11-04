namespace MatrixMultiplicationTest;

using MatrixMultiplication;
using MatrixMultiplication.Strategies;
using MatrixMultiplication.Generators;
using MatrixMultiplication.Readers;
using MatrixMultiplication.Writers;
using NUnit.Framework;

/// <summary>
/// Tests for Int2DOperations and Int2DArrayToFile... writer and reader.
/// </summary>
[TestFixture]
public class Tests
{
    /// <summary>
    /// path for read and write test.
    /// </summary>
    private const string MatrixReadTestPath = "./MatrixGenTest";

    /// <summary>
    /// Test checking the equality of matrices multiplied in parallel and in series.
    /// </summary>
    /// <param name="leftRowCount">number of rows of the left generated matrix.</param>
    /// <param name="commonCount">number of columns and row of the right and left generated matrix, respectively.</param>
    /// <param name="rightColumnCount">number of columns of the right generated matrix.</param>
    [TestCase(1, 1, 100)]
    [TestCase(1, 100, 1)]
    [TestCase(100, 1, 1)]
    public void ParMulEqualsToSeqMulTest(int leftRowCount, int commonCount, int rightColumnCount)
    {
        var leftArray = IntArrayGenerator.Generate2DIntArray(leftRowCount, commonCount);
        var rightArray = IntArrayGenerator.Generate2DIntArray(commonCount, rightColumnCount);

        var sequentialStrategy = new SequentialStrategy();
        var parallelStrategy = new ParallelStrategy();

        var leftMatrix = new IntMatrix(leftArray);
        var rightMatrix = new IntMatrix(rightArray);

        var parallelResult = leftMatrix.MultiplyWithStrategy(rightMatrix, parallelStrategy).ToArray;
        var sequentialResult = leftMatrix.MultiplyWithStrategy(rightMatrix, sequentialStrategy).ToArray;

        Assert.That(parallelResult, Is.EqualTo(sequentialResult));
    }

    /// <summary>
    /// Test that checks the correctness of the dimension in parallel multiplication.
    /// </summary>
    /// <param name="leftRowCount">number of rows of the left generated matrix.</param>
    /// <param name="commonCount">number of columns and row of the right and left generated matrix, respectively.</param>
    /// <param name="rightColumnCount">number of columns of the right generated matrix.</param>
    [TestCase(1, 1, 100)]
    [TestCase(1, 100, 1)]
    [TestCase(100, 1, 1)]
    public void ParMulCorrectMatrixDimTest(int leftRowCount, int commonCount, int rightColumnCount)
    {
        var leftArray = IntArrayGenerator.Generate2DIntArray(leftRowCount, commonCount);
        var rightArray = IntArrayGenerator.Generate2DIntArray(commonCount, rightColumnCount);

        var strategy = new ParallelStrategy();

        var leftMatrix = new IntMatrix(leftArray);
        var rightMatrix = new IntMatrix(rightArray);

        var parallelResult = leftMatrix.MultiplyWithStrategy(rightMatrix, strategy);

        Assert.AreEqual(leftRowCount, parallelResult.GetLength(0));
        Assert.AreEqual(rightColumnCount, parallelResult.GetLength(1));
    }

    /// <summary>
    /// Test that checks the correctness of the dimension in sequential multiplication.
    /// </summary>
    /// <param name="leftRowCount">Number of rows of the left generated matrix.</param>
    /// <param name="commonCount">Number of columns and row of the right and left generated matrix, respectively.</param>
    /// <param name="rightColumnCount">Number of columns of the right generated matrix.</param>
    [TestCase(1, 1, 100)]
    [TestCase(1, 100, 1)]
    [TestCase(100, 1, 1)]
    public void SeqMulCorrectMatrixDimTest(int leftRowCount, int commonCount, int rightColumnCount)
    {
        var leftArray = IntArrayGenerator.Generate2DIntArray(leftRowCount, commonCount);
        var rightArray = IntArrayGenerator.Generate2DIntArray(commonCount, rightColumnCount);

        var strategy = new SequentialStrategy();

        var leftMatrix = new IntMatrix(leftArray);
        var rightMatrix = new IntMatrix(rightArray);

        var result = leftMatrix.MultiplyWithStrategy(rightMatrix, strategy);

        Assert.AreEqual(leftRowCount, result.GetLength(0));
        Assert.AreEqual(rightColumnCount, result.GetLength(1));
    }

    /// <summary>
    /// Read and write reversibility test.
    /// </summary>
    /// <param name="rowCount">Number of rows of the generated matrix.</param>
    /// <param name="columnCount">Number of columns of the generated matrix.</param>
    [TestCase(1, 100)]
    [TestCase(100, 1)]
    [TestCase(10000, 1000)]
    [TestCase(1, 1)]
    public void CorrectReadText2DArrayTest(int rowCount, int columnCount)
    {
        var int2DArray = IntArrayGenerator.Generate2DIntArray(100, 1000);

        Int2DArrayToTextFileWriter.Write(MatrixReadTestPath, int2DArray);

        var resultMatrix = TextFileToInt2DArrayReader.Read(MatrixReadTestPath);

        Assert.AreEqual(int2DArray, resultMatrix);
    }

    /// <summary>
    /// Test that checks the dimension of the generated matrices.
    /// </summary>
    /// <param name="rowCount">Number of rows of the generated matrix.</param>
    /// <param name="columnCount">Number of columns of the generated matrix.</param>
    [TestCase(1, 100)]
    [TestCase(100, 1)]
    [TestCase(10000, 1000)]
    [TestCase(1, 1)]
    public void CorrectMatrixGen(int rowCount, int columnCount)
    {
        var int2DArray = IntArrayGenerator.Generate2DIntArray(rowCount, columnCount);

        Assert.AreEqual(rowCount, int2DArray.GetLength(0));
        Assert.AreEqual(columnCount, int2DArray.GetLength(1));
    }
}
