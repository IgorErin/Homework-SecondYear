namespace ThreadPool.Exceptions;

/// <summary>
/// <see cref="MyTask"/> class exception.
/// </summary>
[Serializable]
public class MyTaskException : Exception
{
    public MyTaskException()
    {
    }

    public MyTaskException(string message) : base(message)
    {
    }

    public MyTaskException(string message, Exception inner) : base(message, inner)
    {
    } 
}
