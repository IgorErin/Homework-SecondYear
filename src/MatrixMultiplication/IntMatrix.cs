namespace MatrixMultiplication;

using Strategies;

/// <summary>
/// Abstract class.
/// Base class for implementing int matrices. Wrap over 2D int array.
/// </summary>
public class IntMatrix
{
    private readonly int[,] intArray;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntMatrix"/> class.
    /// </summary>
    /// <param name="intArray">Int 2D array that will be wrapped.</param>
    public IntMatrix(int[,] intArray)
        => this.intArray = intArray;

    /// <summary>
    /// Gets array representation of matrix.
    /// </summary>
    public int[,] ToArray
    {
        get => this.intArray.Clone() as int[,]
               ?? throw new NullReferenceException("Method ToArray trying to return null");
    }

    /// <summary>
    /// Method that allows you to multiply two <see cref="IntMatrix"/>.
    /// </summary>
    /// <param name="rightMatrix">Right matrix for multiplication.</param>
    /// <param name="strategy">Multiplication strategy.</param>
    /// <returns><see cref="IntMatrix"/> that represent result of multiplication.</returns>
    public IntMatrix MultiplyWithStrategy(IntMatrix rightMatrix, IMultiplicationStrategy strategy)
        => new IntMatrix(strategy.Multiply(this.intArray, rightMatrix.intArray));

    /// <summary>
    /// Method returning the dimension by the passed dimension.
    /// </summary>
    /// <param name="index">dimension.</param>
    /// <returns>Int value - dimensions count.</returns>
    public int GetLength(int index)
        => this.intArray.GetLength(index);
}
