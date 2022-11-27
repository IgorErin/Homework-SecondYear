namespace MyNunit.Exceptions;

/// <summary>
/// <see cref="MyNunit"/> success exception.
/// </summary>
[Serializable]
public class SuccessException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessException"/> class.
    /// </summary>
    public SuccessException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public SuccessException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="innerException">Inner exception.</param>
    public SuccessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}