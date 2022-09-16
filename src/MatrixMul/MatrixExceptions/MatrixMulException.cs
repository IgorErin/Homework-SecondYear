using System;

namespace MatrixMul.MatrixExceptions;

[Serializable]
public class MatrixMulException : Exception
{
    public MatrixMulException()
    {
    }

    public MatrixMulException(string message) : base(message)
    {
    }
}
