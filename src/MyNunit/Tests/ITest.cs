namespace MyNunit.Tests;

public interface ITest
{
    public long Time
    {
        get;
    }

    public TestStatus Status
    {
        get;
    }

    public string Message
    {
        get;
    }

    public void Run();
}