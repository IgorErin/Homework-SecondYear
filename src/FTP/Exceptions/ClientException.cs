namespace FTP.Exceptions;

/// <summary>
/// Exception class for <see cref="Client"/>
/// </summary>
public class ClientException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientException"/> class.
    /// </summary>
    public ClientException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public ClientException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="inner">Inner exception.</param>
    public ClientException(string message, Exception inner)
        : base(message, inner)
    {
    }
}