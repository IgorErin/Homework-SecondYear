namespace MatrixMultiplication.MatrixExceptions;

/// <summary>
/// Int matrix exception.
/// </summary>
[Serializable]
public class IntMatrixException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IntMatrixException"/> class.
    /// </summary>
    public IntMatrixException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IntMatrixException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public IntMatrixException(string message)
        : base(message)
    {
    }
}
