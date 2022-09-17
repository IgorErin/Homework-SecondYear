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

    private const string LeftMatrixPath = "test/MatrixMulTest/TextMatrices/leftInt2DArray";
    private const string RightMatrixPath = "test/MatrixMulTest/TextMatrices/rightInt2DArray";

    [Test]
    public void ParMulEqualsToSeqMulTest()
    {
        var leftMatrix = TextFileToInt2DArrayReader.GetMatrix(LeftMatrixPath);
        var rightMatrix = TextFileToInt2DArrayReader.GetMatrix(RightMatrixPath);

        var parResult = MatrixOperations.Int2DArrayParallelMul(leftMatrix, rightMatrix);
        var seqResult = MatrixOperations.Int2DArraySequentialMul(leftMatrix, rightMatrix);
        
        Assert.That(parResult, Is.EqualTo(seqResult));
    }
    
    [Test]
    public void ParMulCorrectMatrixDimTest()
    {
        var leftMatrix = ArrayGenerator.Generate2DIntArray(1, 1000);
        var rightMatrix = ArrayGenerator.Generate2DIntArray(1000, 2);

        var parResult = MatrixOperations.Int2DArrayParallelMul(leftMatrix, rightMatrix);
        
        Console.WriteLine(parResult.GetLength(0));

        Assert.AreEqual(1, parResult.GetLength(0));
        Assert.AreEqual(2, parResult.GetLength(1));
    }
    
    [Test]
    public void SeqMulCorrectMatrixDimTest()
    {
        var leftMatrix = ArrayGenerator.Generate2DIntArray(1, 1000);
        var rightMatrix = ArrayGenerator.Generate2DIntArray(1000, 2);

        var parResult = MatrixOperations.Int2DArraySequentialMul(leftMatrix, rightMatrix);

        Assert.AreEqual(1,parResult.GetLength(0));
        Assert.AreEqual(2, parResult.GetLength(1));
    }
    
    [Test]
    public void CorrectReadText2DArrayTest()
    {
        var int2DArray = ArrayGenerator.Generate2DIntArray(100, 1000);

        Int2DArrayToTextFileWriter.WriteToFile(MatrixReadTestPath, int2DArray);

        var resultMatrix = TextFileToInt2DArrayReader.GetMatrix(MatrixReadTestPath);
        
        Assert.AreEqual(int2DArray , resultMatrix);
    }
}