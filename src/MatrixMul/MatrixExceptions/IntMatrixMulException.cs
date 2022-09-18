using System;

namespace MatrixMul.MatrixExceptions;

/// <summary>
/// Int matrix exception.
/// </summary>
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
