namespace Lazy.LazyExceptions;

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
