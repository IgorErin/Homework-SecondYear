namespace ThreadPool.Exceptions;

/// <summary>
/// <see cref="ComputationCell{TResult}"/> class exception.
/// </summary>
[Serializable]
public class ComputationCellException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComputationCellException"/> class.
    /// </summary>
    public ComputationCellException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComputationCellException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public ComputationCellException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComputationCellException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="inner">Inner exception.</param>
    public ComputationCellException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
