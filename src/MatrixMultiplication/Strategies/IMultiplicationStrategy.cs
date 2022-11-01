namespace MatrixMultiplication.Strategies;

/// <summary>
/// Multiplication strategy interface.
/// </summary>
public interface IMultiplicationStrategy
{
    /// <summary>
    /// Method that performs multiplication.
    /// </summary>
    /// <param name="leftMatrix">Left matrix array representation.</param>
    /// <param name="rightMatrix">Right matrix array representation.</param>
    /// <returns>Result of multiplication.</returns>
    public int[,] Multiply(int[,] leftMatrix, int[,] rightMatrix);
}
