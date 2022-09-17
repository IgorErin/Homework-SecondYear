using MatrixMul.MatrixExceptions;

namespace MatrixMul.Matrices;
public class IntSequentialMatrix : IntMatrix
{
    public IntSequentialMatrix(int[,] intArray) : base(intArray)
    {
    }

    public static IntMatrix operator *(IntSequentialMatrix leftMatrix, IntSequentialMatrix rightMatrix)
    {
        if (NotAvailableForMultiplication(leftMatrix, rightMatrix))
        {
            throw new IntMatrixMulException("matrix multiplication is not possible, wrong dimension");
        }

        var result = Multiply(leftMatrix.IntArray, rightMatrix.IntArray);
        
        return new IntSequentialMatrix(result);
    }

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
