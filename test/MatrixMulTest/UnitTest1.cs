using System;
using MatrixMul;
using MatrixMul.Generators;
using MatrixMul.Readers;
using MatrixMul.Writers;
using NUnit.Framework;

namespace MatrixMulTest;

[TestFixture]
public class Tests
{
    private const string MatrixReadTestPath = "./MatrixGenTest";

    [TestCase(1, 1, 100)]
    [TestCase(1, 100, 1)]
    [TestCase(100, 1, 1)]
    public void ParMulEqualsToSeqMulTest(int leftRowCount, int commonCount, int rightColumnCount)
    {

        var leftMatrix = ArrayGenerator.Generate2DIntArray(leftRowCount, commonCount);
        var rightMatrix = ArrayGenerator.Generate2DIntArray(commonCount, rightColumnCount);

        var parResult = MatrixOperations.Int2DArrayParallelMul(leftMatrix, rightMatrix);
        var seqResult = MatrixOperations.Int2DArraySequentialMul(leftMatrix, rightMatrix);
        
        Assert.That(parResult, Is.EqualTo(seqResult));
    }
    
    [TestCase(1, 1, 100)]
    [TestCase(1, 100, 1)]
    [TestCase(100, 1, 1)]
    public void ParMulCorrectMatrixDimTest(int leftRowCount, int commonCount, int rightColumnCount)
    {
        var leftMatrix = ArrayGenerator.Generate2DIntArray(leftRowCount, commonCount);
        var rightMatrix = ArrayGenerator.Generate2DIntArray(commonCount, rightColumnCount);

        var parResult = MatrixOperations.Int2DArrayParallelMul(leftMatrix, rightMatrix);
        
        Console.WriteLine(parResult.GetLength(0));

        Assert.AreEqual(leftRowCount, parResult.GetLength(0));
        Assert.AreEqual(rightColumnCount, parResult.GetLength(1));
    }
    
    [TestCase(1, 1, 100)]
    [TestCase(1, 100, 1)]
    [TestCase(100, 1, 1)] 
    public void SeqMulCorrectMatrixDimTest(int leftRowCount, int commonCount, int rightColumnCount)
    {
        var leftMatrix = ArrayGenerator.Generate2DIntArray(leftRowCount, commonCount);
        var rightMatrix = ArrayGenerator.Generate2DIntArray(commonCount, rightColumnCount);

        var parResult = MatrixOperations.Int2DArraySequentialMul(leftMatrix, rightMatrix);

        Assert.AreEqual(leftRowCount,parResult.GetLength(0));
        Assert.AreEqual(rightColumnCount, parResult.GetLength(1));
    }
    
    [TestCase(1, 100)]
    [TestCase(100, 1)]
    [TestCase(10000, 1000)]
    [TestCase(1, 1)]
    public void CorrectReadText2DArrayTest(int rowCount, int columnCount)
    {
        var int2DArray = ArrayGenerator.Generate2DIntArray(100, 1000);

        Int2DArrayToTextFileWriter.WriteToFile(MatrixReadTestPath, int2DArray);

        var resultMatrix = TextFileToInt2DArrayReader.GetMatrix(MatrixReadTestPath);
        
        Assert.AreEqual(int2DArray , resultMatrix);
    }
    
    [TestCase(1, 100)]
    [TestCase(100, 1)]
    [TestCase(10000, 1000)]
    [TestCase(1, 1)]
    public void CorrectMatrixGen(int rowCount, int columnCount)
    {
        
        var int2DArray = ArrayGenerator.Generate2DIntArray(rowCount, columnCount);

        Assert.AreEqual(rowCount , int2DArray.GetLength(0));
        Assert.AreEqual(columnCount , int2DArray.GetLength(1));
    }
}