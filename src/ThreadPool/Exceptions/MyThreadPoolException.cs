namespace ThreadPool.Exceptions;

/// <summary>
/// <see cref="MyThreadPool"/> class exception.
/// </summary>
[Serializable]
public class MyThreadPoolException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPoolException"/> class.
    /// </summary>
    public MyThreadPoolException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPoolException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public MyThreadPoolException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPoolException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="inner">Inner exception.</param>
    public MyThreadPoolException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
