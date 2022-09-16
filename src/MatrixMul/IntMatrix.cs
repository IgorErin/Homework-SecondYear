namespace MatrixMul;

public abstract class IntMatrix
{
    protected readonly int[,] IntArray;
    
    private readonly int _rowCount;
    private readonly int _columnCount;

    protected IntMatrix(int[,] intArray)
    {
        this.IntArray = intArray;
        this._rowCount = intArray.GetLength(0);
        this._columnCount = intArray.GetLength(1);
    }

    protected static bool AvailableForMultiplication(IntMatrix leftMatrix, IntMatrix rightMatrix)
    {
        var rowLenLeftMatrix = leftMatrix.IntArray.GetLength(0);
        var columnLenRightMatrix = rightMatrix.IntArray.GetLength(1); //TODO()

        return rowLenLeftMatrix != columnLenRightMatrix;
    }
    
    protected static int GetResultItem(int resultColumnIndex, int resultRowIndex, int[ , ] leftArray, int[ , ] rightArray)
    {
        int sum = 0;
        
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
    
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();

        for (var rowIndex = 0; rowIndex < this._rowCount; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < this._columnCount; columnIndex++)
            {
                sb.Append(this.IntArray[rowIndex, columnIndex].ToString() + " ");
            }

            sb.Append("\n");
        }

        return sb.ToString();
    }
}
