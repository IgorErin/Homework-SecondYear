namespace MyNunit;

using Optional;

public class TestClassInfo
{
    private readonly long time;
    private readonly List<TestInfo> tests;
    private readonly Option<Exception> testRunTimeException;
    private readonly string message;
    private readonly Type type;

    public List<TestInfo> Tests => tests;

    public long Time => this.time;

    public string Message => this.message;

    public Option<Exception> Exception => testRunTimeException;

    public string Name => this.type.Name;

    public TestClassInfo(long time, List<TestInfo> tests, Option<Exception> testRunTimeException, string message, Type type)
    {
        this.time = time;
        this.tests = tests;
        this.testRunTimeException = testRunTimeException;
        this.message = message;
    }

    public TestClassInfo(long time, List<TestInfo> tests, string message, Type type)
        : this(time, tests, Option.None<Exception>(), message, type)
    {
    }

    public TestClassInfo(string message, Type type)
        : this(0, new List<TestInfo>(), message, type)
    {
    }
}