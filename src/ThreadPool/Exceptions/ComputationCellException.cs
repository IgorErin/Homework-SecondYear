namespace ThreadPool.Exceptions;

/// <summary>
/// <see cref="ComputationCell{TResult}"/> class exception.
/// </summary>
[Serializable]
public class ComputationCellException : Exception
{
    public ComputationCellException()
    {
    }

    public ComputationCellException(string message) : base(message)
    {
    }

    public ComputationCellException(string message, Exception inner) : base(message, inner)
    {
    } 
}
