namespace MatrixMultiplication.Writers;

/// <summary>
/// Static class.
/// Writer of a two-dimensional array to a file.
/// </summary>
public static class Int2DArrayToTextFileWriter
{
    /// <summary>
    /// Method that takes a two-dimensional array and writes it to a file at the specified path.
    /// </summary>
    /// <param name="pathToFile">The path to the file, existing or not, where the array will be written.</param>
    /// <param name="matrix">2D array to be written.</param>
    public static void Write(string pathToFile, int[,] matrix)
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
