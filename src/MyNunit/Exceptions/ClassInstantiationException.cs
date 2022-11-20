namespace MyNunit.Exceptions;

[Serializable]
public class ClassInstantiationException : Exception
{
    public ClassInstantiationException()
    {
    }

    public ClassInstantiationException(string message) : base(message)
    {
    }

    public ClassInstantiationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
