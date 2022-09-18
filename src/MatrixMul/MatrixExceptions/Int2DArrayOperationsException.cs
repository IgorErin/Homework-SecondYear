using System;

namespace MatrixMul.MatrixExceptions;

/// <summary>
/// Int 2D array operations exception.
/// </summary>
[Serializable]
public class Int2DArrayOperationsException : Exception
{
    public Int2DArrayOperationsException()
    {
    }

    public Int2DArrayOperationsException(string message) : base(message)
    {
    }
}