namespace Lazy.LazyExceptions;

/// <summary>
/// Exception thrown on mismatch on field access error in <see cref="Lazy{T}"/>.
/// </summary>
[Serializable]
public class NotComputedResultLazyException : Exception
{
    public NotComputedResultLazyException()
    {
    }
    
    public NotComputedResultLazyException(string message) : base(message)
    {
    }

    public NotComputedResultLazyException(string message, Exception inner) : base(message, inner)
    {
    }
}
