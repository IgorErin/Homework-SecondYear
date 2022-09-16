using System;
using System.IO;

namespace MatrixMul.Readers;

public class FileToIntMatrixReader
{
    public static int[ , ] GetMatrix(string pathToFile)
    {
        string[] textLines = File.ReadAllLines(pathToFile);

        return ReadMatrix(textLines);
    }

    private static int[ , ] ReadMatrix(string[] textLines)
    {
        var rowCount = textLines.Length;
        var columnCount = textLines[0].Split(" ").Length;
        
        var matrix = new int[rowCount, columnCount];

        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            var textLine = textLines[rowIndex].Split(" ");
            
            for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                matrix[rowIndex, columnIndex] = Convert.ToInt32(textLine[columnIndex]);
            }
        }

        return matrix;
    }
}