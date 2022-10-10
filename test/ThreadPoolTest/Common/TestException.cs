using System;

namespace ThreadPool.Common;

[Serializable]
public class TestException : Exception
{
    public TestException()
    {
    }

    public TestException(string message) : base(message)
    {
    }

    public TestException(string messge, Exception inner) : base(messge, inner)
    {
    }
}
