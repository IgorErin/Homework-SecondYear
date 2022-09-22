namespace Lazy.LazyExceptions;

/// <summary>
/// Exception thrown on mismatch on status mismatch in <see cref="Lazy{T}"/>.
/// </summary>
[Serializable]
public class ComputedStatusNotMatchLazyException : Exception
{
    public ComputedStatusNotMatchLazyException()
    {
    }
    
    public ComputedStatusNotMatchLazyException(string message) : base(message)
    {
    }

    public ComputedStatusNotMatchLazyException(string message, Exception inner) : base(message, inner)
    {
    }
}