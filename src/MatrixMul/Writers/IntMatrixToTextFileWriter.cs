using MatrixMul.Matrices;

namespace MatrixMul.Writers;

public static class IntMatrixToTextFileWriter
{
    public static void WriteToFile(string pathToFile, IntMatrix matrix)
    {
        using var writer = new StreamWriter(pathToFile);

        var rowCount = matrix.GetLength(0);
        var columnCount = matrix.GetLength(1);

        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < matrix.GetLength(1); columnIndex++)
            {
                writer.Write(matrix[rowIndex, columnIndex]);
                
                if (columnIndex < columnCount - 1)
                {
                    writer.Write(" ");
                }
            }

            writer.Write("\n");
        }
    }
}
