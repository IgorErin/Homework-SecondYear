namespace Lazy.LazyExceptions;

/// <summary>
/// Exception thrown on mismatch on field access error in <see cref="Lazy{T}"/>.
/// </summary>
[Serializable]
public class NotComputedValueLazyException : Exception
{
    public NotComputedValueLazyException()
    {
    }
    
    public NotComputedValueLazyException(string message) : base(message)
    {
    }

    public NotComputedValueLazyException(string message, Exception inner) : base(message, inner)
    {
    }
}
