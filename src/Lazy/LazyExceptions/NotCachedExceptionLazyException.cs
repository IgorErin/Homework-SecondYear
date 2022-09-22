namespace Lazy.LazyExceptions;

/// <summary>
/// Exception thrown on mismatch on exception cache error in <see cref="Lazy{T}"/>.
/// </summary>
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