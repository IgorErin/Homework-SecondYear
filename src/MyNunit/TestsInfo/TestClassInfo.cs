namespace MyNunit.TestsInfo;

using System.Reflection;
using System.Text;
using Optional;

/// <summary>
/// Class representing information about the results of class tests.
/// </summary>
public class TestClassInfo
{
    private readonly long time;
    private readonly List<TestInfo> tests;
    private readonly Option<Exception> testRuntimeException;
    private readonly string message;
    private readonly TypeInfo typeInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestClassInfo"/> class.
    /// </summary>
    /// <param name="time">Test execution time.</param>
    /// <param name="tests">Type test data.</param>
    /// <param name="testRuntimeException">Test runtime exception.</param>
    /// <param name="message">Result message.</param>
    /// <param name="typeInfo">Information about the type of the tested class.</param>
    public TestClassInfo(long time, List<TestInfo> tests, Option<Exception> testRuntimeException, string message, TypeInfo typeInfo)
    {
        this.time = time;
        this.tests = tests;
        this.testRuntimeException = testRuntimeException;
        this.message = message;
        this.typeInfo = typeInfo;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestClassInfo"/> class.
    /// </summary>
    /// <param name="time">Test execution time.</param>
    /// <param name="tests">Type test data.</param>
    /// <param name="message">Result message.</param>
    /// <param name="type">Information about the type of the tested class.</param>
    public TestClassInfo(long time, List<TestInfo> tests, string message, TypeInfo type)
        : this(time, tests, Option.None<Exception>(), message, type)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestClassInfo"/> class.
    /// </summary>
    /// <param name="message">Result message.</param>
    /// <param name="type">Information about the type of the tested class.</param>
    public TestClassInfo(string message, TypeInfo type)
        : this(0, new List<TestInfo>(), message, type)
    {
    }

    /// <summary>
    /// Gets class tests information.
    /// </summary>
    public List<TestInfo> Tests => this.tests;

    /// <summary>
    /// Gets class test time.
    /// </summary>
    public long Time => this.time;

    /// <summary>
    /// Gets class test message.
    /// </summary>
    public string Message => this.message;

    /// <summary>
    /// Gets test exception.
    /// </summary>
    public Option<Exception> Exception => this.testRuntimeException;

    /// <summary>
    /// Gets class name.
    /// </summary>
    public string Name => this.typeInfo.Name;

    /// <summary>
    /// <see cref="ToString"/> method.
    /// </summary>
    /// <returns>Class test information in string form.</returns>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"Test class: {this.Name}");
        stringBuilder.AppendLine($"Status: {this.message}");
        stringBuilder.AppendLine($"Type test time: {this.time} milliseconds");

        this.testRuntimeException.Match(
            some: value => stringBuilder.AppendLine($"Exception: {value.Message}"),
            none: () => { });

        stringBuilder.AppendLine();

        foreach (var test in this.tests)
        {
            stringBuilder.Append(test);
        }

        return stringBuilder.ToString();
    }
}
