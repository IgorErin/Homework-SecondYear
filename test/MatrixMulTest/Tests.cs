using System;
using MatrixMul;
using MatrixMul.Generators;
using MatrixMul.Matrices;
using MatrixMul.Readers;
using MatrixMul.Writers;
using NUnit.Framework;

namespace MatrixMulTest;

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
    /// Test checking the equality of matrices multiplied in parallel and in series
    /// </summary>
    /// <param name="leftRowCount">number of rows of the left generated matrix</param>
    /// <param name="commonCount">number of columns and row of the right and left generated matrix, respectively</param>
    /// <param name="rightColumnCount">number of columns of the right generated matrix</param>

    [TestCase(1, 1, 100)]
    [TestCase(1, 100, 1)]
    [TestCase(100, 1, 1)]
    public void ParMulEqualsToSeqMulTest(int leftRowCount, int commonCount, int rightColumnCount)
    {
        var leftMatrix = IntArrayGenerator.Generate2DIntArray(leftRowCount, commonCount);
        var rightMatrix = IntArrayGenerator.Generate2DIntArray(commonCount, rightColumnCount);

        var parResult = Int2DArrayOperations.Int2DArrayParallelMul(leftMatrix, rightMatrix);
        var seqResult = Int2DArrayOperations.Int2DArraySequentialMul(leftMatrix, rightMatrix);
        
        Assert.That(parResult, Is.EqualTo(seqResult));
    }
    
    /// <summary>
    /// Test that checks the correctness of the dimension in parallel multiplication
    /// </summary>
    /// <param name="leftRowCount">number of rows of the left generated matrix</param>
    /// <param name="commonCount">number of columns and row of the right and left generated matrix, respectively</param>
    /// <param name="rightColumnCount">number of columns of the right generated matrix</param>
    [TestCase(1, 1, 100)]
    [TestCase(1, 100, 1)]
    [TestCase(100, 1, 1)]
    public void ParMulCorrectMatrixDimTest(int leftRowCount, int commonCount, int rightColumnCount)
    {
        var leftMatrix = IntArrayGenerator.Generate2DIntArray(leftRowCount, commonCount);
        var rightMatrix = IntArrayGenerator.Generate2DIntArray(commonCount, rightColumnCount);

        var parResult = Int2DArrayOperations.Int2DArrayParallelMul(leftMatrix, rightMatrix);
        
        Console.WriteLine(parResult.GetLength(0));

        Assert.AreEqual(leftRowCount, parResult.GetLength(0));
        Assert.AreEqual(rightColumnCount, parResult.GetLength(1));
    }
    
    /// <summary>
    /// Test that checks the correctness of the dimension in sequential multiplication
    /// </summary>
    /// <param name="leftRowCount">Number of rows of the left generated matrix</param>
    /// <param name="commonCount">Number of columns and row of the right and left generated matrix, respectively</param>
    /// <param name="rightColumnCount">Number of columns of the right generated matrix</param>
    [TestCase(1, 1, 100)]
    [TestCase(1, 100, 1)]
    [TestCase(100, 1, 1)] 
    public void SeqMulCorrectMatrixDimTest(int leftRowCount, int commonCount, int rightColumnCount)
    {
        var leftMatrix = IntArrayGenerator.Generate2DIntArray(leftRowCount, commonCount);
        var rightMatrix = IntArrayGenerator.Generate2DIntArray(commonCount, rightColumnCount);

        var parResult = Int2DArrayOperations.Int2DArraySequentialMul(leftMatrix, rightMatrix);

        Assert.AreEqual(leftRowCount,parResult.GetLength(0));
        Assert.AreEqual(rightColumnCount, parResult.GetLength(1));
    }
    
    /// <summary>
    /// Read and write reversibility test
    /// </summary>
    /// <param name="rowCount">Number of rows of the generated matrix</param>
    /// <param name="columnCount">Number of columns of the generated matrix</param>
    [TestCase(1, 100)]
    [TestCase(100, 1)]
    [TestCase(10000, 1000)]
    [TestCase(1, 1)]
    public void CorrectReadText2DArrayTest(int rowCount, int columnCount)
    {
        var int2DArray = IntArrayGenerator.Generate2DIntArray(100, 1000);

        Int2DArrayToTextFileWriter.Write(MatrixReadTestPath, int2DArray);

        var resultMatrix = TextFileToInt2DArrayReader.Read(MatrixReadTestPath);
        
        Assert.AreEqual(int2DArray , resultMatrix);
    }
    
    /// <summary>
    /// Test that checks the dimension of the generated matrices
    /// </summary>
    /// <param name="rowCount">Number of rows of the generated matrix</param>
    /// <param name="columnCount">Number of columns of the generated matrix</param>
    [TestCase(1, 100)]
    [TestCase(100, 1)]
    [TestCase(10000, 1000)]
    [TestCase(1, 1)]
    public void CorrectMatrixGen(int rowCount, int columnCount)
    {
        
        var int2DArray = IntArrayGenerator.Generate2DIntArray(rowCount, columnCount);

        Assert.AreEqual(rowCount , int2DArray.GetLength(0));
        Assert.AreEqual(columnCount , int2DArray.GetLength(1));
    }
    
    /// <summary>
    /// Test that checks the equals of the array operations and IntMatrix parallel multiplication
    /// </summary>
    /// <param name="leftRowCount">Number of rows of the left generated matrix</param>
    /// <param name="commonCount">Number of columns and row of the right and left generated matrix, respectively</param>
    /// <param name="rightColumnCount">Number of columns of the right generated matrix</param>
    [TestCase(1, 1, 100)]
    [TestCase(1, 100, 1)]
    [TestCase(100, 1, 1)]
    public void OperationAndIntMatrixParallelMultiplyAreEqual(int leftRowCount, int commonCount, int rightColumnCount)
    {
        
        var leftArray = IntArrayGenerator.Generate2DIntArray(leftRowCount, commonCount);
        var rightArray = IntArrayGenerator.Generate2DIntArray(commonCount, rightColumnCount);

        var leftIntMatrix = new IntParallelMatrix(leftArray);
        var rightIntMatrix = new IntParallelMatrix(rightArray);

        var intMatrixResult = leftIntMatrix * rightIntMatrix;
        var operationsResult = Int2DArrayOperations.Int2DArraySequentialMul(leftArray, rightArray);

        for (var i = 0; i < leftRowCount; i++)
        {
            for (var j = 0; j < rightColumnCount; j++)
            {
                Assert.AreEqual(operationsResult[i, j], intMatrixResult[i, j]);
            }
        }
    }
    
    
    /// <summary>
    /// Test that checks the equals of the array operations and IntMatrix sequential multiplication
    /// </summary>
    /// <param name="leftRowCount">Number of rows of the left generated matrix</param>
    /// <param name="commonCount">Number of columns and row of the right and left generated matrix, respectively</param>
    /// <param name="rightColumnCount">Number of columns of the right generated matrix</param>
    [TestCase(1, 1, 100)]
    [TestCase(1, 100, 1)]
    [TestCase(100, 1, 1)]
    public void OperationAndIntMatrixSequentialMultiplyAreEqual(int leftRowCount, int commonCount, int rightColumnCount)
    {
        
        var leftArray = IntArrayGenerator.Generate2DIntArray(leftRowCount, commonCount);
        var rightArray = IntArrayGenerator.Generate2DIntArray(commonCount, rightColumnCount);

        var leftIntMatrix = new IntSequentialMatrix(leftArray);
        var rightIntMatrix = new IntSequentialMatrix(rightArray);

        var intMatrixResult = leftIntMatrix * rightIntMatrix;
        var operationsResult = Int2DArrayOperations.Int2DArraySequentialMul(leftArray, rightArray);

        for (var i = 0; i < leftRowCount; i++)
        {
            for (var j = 0; j < rightColumnCount; j++)
            {
                Assert.AreEqual(operationsResult[i, j], intMatrixResult[i, j]);
            }
        }
    }
}