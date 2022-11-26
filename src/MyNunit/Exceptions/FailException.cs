namespace MyNunit.Exceptions;

/// <summary>
/// <see cref="MyNunit"/> fail exception.
/// </summary>
[Serializable]
public class FailException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailException"/> class.
    /// </summary>
    public FailException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FailException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public FailException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FailException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="innerException">Inner exception.</param>
    public FailException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
