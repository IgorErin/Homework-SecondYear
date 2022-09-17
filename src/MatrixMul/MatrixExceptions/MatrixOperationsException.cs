namespace MatrixMul.MatrixExceptions;

[Serializable]
public class MatrixOperationsException : Exception
{
    public MatrixOperationsException()
    {
    }

    public MatrixOperationsException(string message) : base(message)
    {
    }
}