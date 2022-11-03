namespace Lazy.ComputationCellExceptions;

/// <summary>
/// Exception thrown on mismatch on exception cache error in <see cref="Lazy{T}"/>.
/// </summary>
[Serializable]
public class NotCachedResultException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotCachedResultException"/> class.
    /// </summary>
    public NotCachedResultException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotCachedResultException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public NotCachedResultException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotCachedResultException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="inner">Inner exception.</param>
    public NotCachedResultException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
