namespace ThreadPool.Exceptions;

/// <summary>
/// <see cref="ResultCell.ResultCell{TResult}"/> class exception.
/// </summary>
[Serializable]
public class ResultCellException : Exception
{
    public ResultCellException()
    {
    }

    public ResultCellException(string message) : base(message)
    {
    }

    public ResultCellException(string message, Exception inner) : base(message, inner)
    {
    } 
}
