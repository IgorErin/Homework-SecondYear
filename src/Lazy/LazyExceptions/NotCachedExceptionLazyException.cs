namespace Lazy.LazyExceptions;

[Serializable]
public class NotCachedExceptionLazyException : Exception
{
    public NotCachedExceptionLazyException()
    {
    }

    public NotCachedExceptionLazyException(string message) : base(message)
    {
    }

    public NotCachedExceptionLazyException(string message, Exception inner) : base(message, inner)
    {
    }
}