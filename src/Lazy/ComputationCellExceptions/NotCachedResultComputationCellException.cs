namespace Lazy.ComputationCellExceptions;

/// <summary>
/// Exception thrown on mismatch on exception cache error in <see cref="Lazy{T}"/>.
/// </summary>
[Serializable]
public class NotCachedResultComputationCellException : Exception
{
    public NotCachedResultComputationCellException()
    {
    }

    public NotCachedResultComputationCellException(string message) : base(message)
    {
    }

    public NotCachedResultComputationCellException(string message, Exception inner) : base(message, inner)
    {
    }
}
