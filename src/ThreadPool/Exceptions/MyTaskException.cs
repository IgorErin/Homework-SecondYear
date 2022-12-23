namespace ThreadPool.Exceptions;

/// <summary>
/// <see cref="MyThreadPool.MyTask{TResult}"/> class exception.
/// </summary>
[Serializable]
public class MyTaskException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyTaskException"/> class.
    /// </summary>
    public MyTaskException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTaskException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public MyTaskException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTaskException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="inner">Inner exception.</param>
    public MyTaskException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
