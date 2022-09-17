namespace MatrixMul.Generators;

public static class MatrixGenerator
{
    private const int MaxRandValue = 20;

    public static int[,] Generate2DIntArray(int rowCount, int columnCount)
    {
        var random = new Random();
        var new2DArray = new int[rowCount, columnCount];

        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                new2DArray[rowIndex, columnIndex] = random.Next(MaxRandValue);
            }
        }

        return new2DArray;
    }
}