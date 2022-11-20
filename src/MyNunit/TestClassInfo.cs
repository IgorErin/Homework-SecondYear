namespace MyNunit;

using System.Reflection;
using System.Text;
using Optional;

public class TestClassInfo
{
    private readonly long time;
    private readonly List<TestInfo> tests;
    private readonly Option<Exception> testRunTimeException;
    private readonly string message;
    private readonly TypeInfo typeInfo;

    public List<TestInfo> Tests => tests;

    public long Time => this.time;

    public string Message => this.message;

    public Option<Exception> Exception => testRunTimeException;

    public string Name => this.typeInfo.Name;

    public TestClassInfo(long time, List<TestInfo> tests, Option<Exception> testRunTimeException, string message, TypeInfo typeInfo)
    {
        this.time = time;
        this.tests = tests;
        this.testRunTimeException = testRunTimeException;
        this.message = message;
        this.typeInfo = typeInfo;
    }

    public TestClassInfo(long time, List<TestInfo> tests, string message, TypeInfo type)
        : this(time, tests, Option.None<Exception>(), message, type)
    {
    }

    public TestClassInfo(string message, TypeInfo type)
        : this(0, new List<TestInfo>(), message, type)
    {
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"Test class: {this.Name}");
        stringBuilder.AppendLine($"Status: {this.message}");
        stringBuilder.AppendLine($"Time: {this.time}");

        this.testRunTimeException.Match(
            some: value => stringBuilder.AppendLine($"Exception: {value.Message}"),
            none: () => {});

        stringBuilder.AppendLine();

        foreach (var test in tests)
        {
            stringBuilder.Append(test.ToString());
        }

        return stringBuilder.ToString();
    }
}
