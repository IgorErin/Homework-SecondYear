namespace MyNunit.TestsInfo;

using System.Reflection;
using System.Text;
using Optional;

/// <summary>
/// Class representing information about the results of class tests.
/// </summary>
public class TestClassInfo
{
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
        this.Time = time;
        this.Tests = tests;
        this.Exception = testRuntimeException;
        this.Message = message;
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
    public List<TestInfo> Tests
    {
        get;
    }

    /// <summary>
    /// Gets class test time.
    /// </summary>
    public long Time
    {
        get;
    }

    /// <summary>
    /// Gets class test message.
    /// </summary>
    public string Message
    {
        get;
    }

    /// <summary>
    /// Gets test exception.
    /// </summary>
    public Option<Exception> Exception
    {
        get;
    }

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
        stringBuilder.AppendLine($"Status: {this.Message}");
        stringBuilder.AppendLine($"Type test time: {this.Time} milliseconds");

        this.Exception.Match(
            some: value => stringBuilder.AppendLine($"Exception: {value.Message}"),
            none: () => { });

        stringBuilder.AppendLine();

        foreach (var test in this.Tests)
        {
            stringBuilder.Append(test);
        }

        return stringBuilder.ToString();
    }
}