namespace MatrixMultiplication.MatrixExceptions;

/// <summary>
/// Int matrix exception.
/// </summary>
[Serializable]
public class IntMatrixMultiplicationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IntMatrixMultiplicationException"/> class.
    /// </summary>
    public IntMatrixMultiplicationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IntMatrixMultiplicationException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public IntMatrixMultiplicationException(string message)
        : base(message)
    {
    }
}
