using System.Security.Principal;
using MatrixMul.MatrixExceptions;

namespace MatrixMul.Matrices;

public abstract class IntMatrix
{
    protected readonly int[,] IntArray;
    
    private readonly int _rowCount;
    private readonly int _columnCount;

    protected IntMatrix(int[,] intArray)
    {
        IntArray = intArray;
        _rowCount = intArray.GetLength(0);
        _columnCount = intArray.GetLength(1);
    }

    protected static bool NotAvailableForMultiplication(IntMatrix leftMatrix, IntMatrix rightMatrix)
    {
        var rowLenLeftMatrix = leftMatrix.IntArray.GetLength(1);
        var columnLenRightMatrix = rightMatrix.IntArray.GetLength(1); 
        
        Console.WriteLine($"left : {rowLenLeftMatrix}, right: {columnLenRightMatrix}");

        return rowLenLeftMatrix != columnLenRightMatrix;
    }
    
    protected static int GetResultItem(
        int resultColumnIndex, 
        int resultRowIndex, 
        int[ , ] leftArray,
        int[ , ] rightArray) 
    {
        var sum = 0;
        
        var leftColumnCount = leftArray.GetLength(1);
        var rightRowCount = rightArray.GetLength(0);

        
        for (var currentColumnIndex = 0; currentColumnIndex < leftColumnCount; currentColumnIndex++)
        {
            for (var currentRowIndex = 0; currentRowIndex < rightRowCount; currentRowIndex++)
            {
                var currentValue = leftArray[resultColumnIndex, currentColumnIndex] *
                                   rightArray[currentRowIndex, resultRowIndex];
                sum += currentValue;
            }
        }

        return sum;
    }

    public static IntMatrix operator *(IntMatrix leftMatrix, IntMatrix rightMatrix)
    {
        throw new IntMatrixMulException("operation \"*\" not implemented yet");
    }
    
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();

        for (var rowIndex = 0; rowIndex < _rowCount; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < this._columnCount; columnIndex++)
            {
                sb.Append(IntArray[rowIndex, columnIndex] + " ");
            }

            sb.Append("\n");
        }

        return sb.ToString();
    }

    public int this[int i, int j]
        => IntArray[i, j];

    public int GetLength(int index)
        => IntArray.GetLength(index);
}
