using MatrixMul.MatrixExceptions;

namespace MatrixMul.Matrices;

/// <summary>
/// Base class implementation of IntMatrix with sequential multiplication.
/// </summary>
public class IntSequentialMatrix : IntMatrix
{
    /// <summary>
    /// Constructor initializing the base class.
    /// </summary>
    /// <param name="intArray">Base class constructor parameter, int 2D array matrix representation</param>
    public IntSequentialMatrix(int[,] intArray) : base(intArray)
    {
    }

    /// <summary>
    /// Method of sequential multiplication of two IntParallel matrices. 
    /// </summary>
    /// <param name="leftMatrix">IntSequential matrix stand on the left when multiplied</param>
    /// <param name="rightMatrix">IntSequential matrix stand on the right when multiplied</param>
    /// <returns>IntParallel matrix - sequential multiplication result</returns>
    /// <exception cref="IntMatrixMulException">
    /// An exception will be thrown when matrices have dimensions that are incorrect for multiplication
    /// </exception>
    public static IntMatrix operator *(IntSequentialMatrix leftMatrix, IntSequentialMatrix rightMatrix)
    {
        if (AvailableForMultiplication(leftMatrix, rightMatrix))
        {
            throw new IntMatrixMulException("matrix multiplication is not possible, wrong dimension");
        }

        var result = Multiply(leftMatrix.IntArray, rightMatrix.IntArray);
        
        return new IntSequentialMatrix(result);
    }

    /// <summary>
    /// Method of sequential multiplication of two int 2D arrays. 
    /// </summary>
    /// <param name="leftArray">Int 2D array stand on the left when multiplied</param>
    /// <param name="rightArray">Int 2D array stand on the right when multiplied</param>
    /// <returns>Result int 2D array</returns>
    private static int[,] Multiply(int[,] leftArray, int[,] rightArray)
    {
        var leftRowCount = leftArray.GetLength(0);
        var rightColumnCount = rightArray.GetLength(1);

        var result = new int[leftRowCount, rightColumnCount];

        for (var resultRowIndex = 0; resultRowIndex < leftRowCount; resultRowIndex++)
        {
            for (var resultColumnIndex = 0; resultColumnIndex < rightColumnCount; resultColumnIndex++)
            {
                var item = GetResultItem(resultColumnIndex, resultRowIndex, leftArray, rightArray);
                
                result[resultRowIndex, resultColumnIndex] = item;
            }
        }

        return result;
    }
}
