namespace MyNunit.Exceptions;

/// <summary>
/// Class instantiation exception
/// </summary>
[Serializable]
public class ClassInstantiationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClassInstantiationException"/> class.
    /// </summary>
    public ClassInstantiationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassInstantiationException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public ClassInstantiationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassInstantiationException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="innerException">Inner exception.</param>
    public ClassInstantiationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
