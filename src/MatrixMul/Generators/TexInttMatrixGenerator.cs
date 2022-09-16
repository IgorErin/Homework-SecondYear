using System;
using System.IO;

namespace MatrixMul.Generators;

public class TextIntMatrixGenerator
{
    private const int MaxRandValue = 1000;
    
    public static void GenerateIntMatrixToTextFile(int rowCount, int columnCount, string pathToFile)
    {
        using var writer = new StreamWriter(File.Create(pathToFile));
        var random = new Random();
        
        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                writer.Write(random.Next(MaxRandValue));
                
                if (columnIndex < columnCount - 1)
                {
                    writer.Write(" ");
                }
            }

            writer.Write("\n");
        }
    }
    
    private static void WriteToFile(string pathToFile, int[,] matrix)
    {
        var rowCount = matrix.GetLength(0);
        var columnCount = matrix.GetLength(1);
        
        using var writer = new StreamWriter(pathToFile);

        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                writer.Write(matrix[rowIndex, columnIndex]);
                
                if (columnIndex < columnIndex - 1)
                {
                    writer.Write(" ");
                }
            }

            writer.Write("\n");
        }
    }
}