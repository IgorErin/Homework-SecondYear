namespace FTP.Exceptions;

/// <summary>
/// Exception class for <see cref="Server"/>
/// </summary>
public class ServerException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerException"/> class.
    /// </summary>
    public ServerException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public ServerException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="inner">Inner exception.</param>
    public ServerException(string message, Exception inner)
        : base(message, inner)
    {
    }
}