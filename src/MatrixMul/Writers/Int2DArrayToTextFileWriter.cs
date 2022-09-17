using MatrixMul.Matrices;

namespace MatrixMul.Writers;

public static class Int2DArrayToTextFileWriter
{
    public static void WriteToFile(string pathToFile, int[,] matrix)
    {
        using var writer = File.CreateText(pathToFile);;

        var rowCount = matrix.GetLength(0);
        var columnCount = matrix.GetLength(1);

        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
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
