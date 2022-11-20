namespace MyNunit.Exceptions;

[Serializable]
public class SuccessException : Exception
{
    public SuccessException()
    {
    }

    public SuccessException(string message) : base(message)
    {
    }

    public SuccessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}