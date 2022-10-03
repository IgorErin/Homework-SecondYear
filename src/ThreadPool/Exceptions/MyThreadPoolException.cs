namespace ThreadPool.Exceptions;

/// <summary>
/// <see cref="MyThreadPool"/> class exception.
/// </summary>
[Serializable]
public class MyThreadPoolException : Exception
{
    public MyThreadPoolException()
    {
    }

    public MyThreadPoolException(string message) : base(message)
    {
    }

    public MyThreadPoolException(string message, Exception inner) : base(message, inner)
    {
    } 
}
