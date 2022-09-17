using System;

namespace MatrixMul.MatrixExceptions;

[Serializable]
public class IntMatrixMulException : Exception
{
    public IntMatrixMulException()
    {
    }

    public IntMatrixMulException(string message) : base(message)
    {
    }
}
