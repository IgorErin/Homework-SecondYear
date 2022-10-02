namespace ThreadPool.Exceptions;

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