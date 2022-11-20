namespace MyNunit;

using Optional;

public class TestClassInfo
{
    private readonly long time;
    private List<TestInfo> tests;
    private Option<Exception> testRunTimeException;

    public List<TestInfo> Tests => tests;

    public long Time => Time;

    public TestClassInfo(long time, List<TestInfo> tests, Option<Exception> exception)
    {
        this.time = time;
        this.tests = tests;
        this.testRunTimeException = exception;
    }

    public TestClassInfo(long time, List<TestInfo> tests) : this(time, tests, Option.None<Exception>())
    {
    }
}