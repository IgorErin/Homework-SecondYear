namespace MatrixMultiplication.Readers;

/// <summary>
/// Static class.
/// 2D int array reader from file.
/// </summary>
public static class TextFileToInt2DArrayReader
{
    /// <summary>
    /// Method accepting the path to the file from which the int array will be read and returning the int array.
    /// </summary>
    /// <param name="pathToFile">Path to the file from where the init array will be read.</param>
    /// <returns>Read two-dimensional int array.</returns>
    public static int[,] Read(string pathToFile)
    {
        var textLines = File.ReadAllLines(pathToFile);

        return ReadMatrix(textLines);
    }

    /// <summary>
    /// Method that extracts an int array from strings.
    /// </summary>
    /// <param name="textLines">An array of strings containing the rows of the array.</param>
    /// <returns>2D int array extracted from array of strings.</returns>
    private static int[,] ReadMatrix(string[] textLines)
    {
        var rowCount = textLines.Length;
        var columnCount = textLines[0].Split(" ").Length;

        var matrix = new int[rowCount, columnCount];

        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            var textLine = textLines[rowIndex].Split(" ");

            if (textLine.Length != columnCount)
            {
                throw new ArgumentException("the input data structure does not match the matrix");
            }

            for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                matrix[rowIndex, columnIndex] = Convert.ToInt32(textLine[columnIndex]);
            }
        }

        return matrix;
    }
}
