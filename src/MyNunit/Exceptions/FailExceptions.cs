namespace MyNunit.Exceptions;

[Serializable]
public class FailExceptions : Exception
{
    public FailExceptions()
    {
    }

    public FailExceptions(string message) : base(message)
    {
    }

    public FailExceptions(string message, Exception innerException) : base(message, innerException)
    {
    }
}
