using MatrixMul.MatrixExceptions;

namespace MatrixMul.Matrices;

/// <summary>
/// Abstract class.
/// Base class for implementing int matrices. Wrap over 2D int array.
/// </summary>
public abstract class IntMatrix
{
    /// <summary>
    /// Representation of a matrix as a two-dimensional array.
    /// </summary>
    protected readonly int[,] IntArray;
    
    /// <summary>
    /// 2D array row count and column count.
    /// </summary>
    private readonly int _rowCount;
    private readonly int _columnCount;

    /// <summary>
    /// Constructor initializing a two-dimensional array and its dimensions.
    /// </summary>
    /// <param name="intArray">Int 2D array that will be wrapped</param>
    protected IntMatrix(int[,] intArray)
    {
        IntArray = intArray;
        _rowCount = intArray.GetLength(0);
        _columnCount = intArray.GetLength(1);
    }

    /// <summary>
    /// A method that checks the correctness of the dimensions of two int matrices for multiplication.
    /// </summary>
    /// <param name="leftMatrix">A matrix that will stand on the left when multiplied.</param>
    /// <param name="rightMatrix">A matrix that will stand on the right when multiplied.</param>
    /// <returns>Boolean equal to true if matrices can be multiplied, otherwise false</returns>
    protected static bool AvailableForMultiplication(IntMatrix leftMatrix, IntMatrix rightMatrix)
    {
        var rowLenLeftMatrix = leftMatrix.GetLength(1);
        var columnLenRightMatrix = rightMatrix.GetLength(0);

        return rowLenLeftMatrix == columnLenRightMatrix;
    }
    
    /// <summary>
    /// Scalar multiplication of row and column of a matrix
    /// </summary>
    ///  <param name="resultRowIndex">left matrix row index</param>
    /// <param name="resultColumnIndex">right matrix column index</param>
    /// <param name="leftArray">The left array whose row will be multiplied</param>
    /// <param name="rightArray">The left array whose column will be multiplied</param>
    /// <returns>Int value - result of scalar multiplication</returns>
    protected static int GetResultItem(
        int resultRowIndex, 
        int resultColumnIndex,
        int[,] leftArray,
        int[,] rightArray) 
    {
        var sum = 0;
        
        var commonCount = leftArray.GetLength(1);

        
        for (var currentIndex = 0; currentIndex < commonCount; currentIndex++)
        {
            var currentValue = leftArray[resultRowIndex, currentIndex] * rightArray[currentIndex, resultColumnIndex];
            sum += currentValue;
        }

        return sum;
    }
    
    /// <summary>
    /// Method returns string representation of matrix.
    /// </summary>
    /// <returns>String value - matrix representation</returns>
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

    /// <summary>
    /// Method returning (i, j) matrix element
    /// </summary>
    /// <param name="i">Row index</param>
    /// <param name="j">Column index</param>
    public int this[int i, int j]
        => IntArray[i, j];

    /// <summary>
    /// Method returning the dimension by the passed dimension.
    /// </summary>
    /// <param name="index">dimension</param>
    /// <returns>Int value - dimensions count</returns>
    public int GetLength(int index)
        => IntArray.GetLength(index);
}
