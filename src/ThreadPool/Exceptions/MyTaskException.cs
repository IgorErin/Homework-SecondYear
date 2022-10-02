namespace ThreadPool.Exceptions;

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
