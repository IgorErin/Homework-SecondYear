namespace MyNunit;

using System.Reflection;
using System.Text;
using Exceptions;
using Extensions;
using Optional;
using Optional.Unsafe;

public class TestInfo
{
    private readonly MethodInfo methodInfo;
    private readonly Exception exceptionResult;
    private readonly Option<Type> expectedException;
    private readonly Option<string> ignoreMessage;
    private readonly long time;

    public string Name => this.methodInfo.Name;

    public TestStatus Status
    {
        get
        {
            if (this.ignoreMessage.HasValue)
            {
                return TestStatus.Ignored;
            }

            if (IsPassedWithExpectedException())
            {
                return TestStatus.Passed;
            }

            return TestStatus.Failed;
        }
    }

    public string Message
    {
        get
        {
            if (this.ignoreMessage.HasValue)
            {
                return $"Ignore, message: {ignoreMessage.ValueOrFailure()}";
            }

            if (IsPassedWithExpectedException())
            {
                return $"Passed with expected exception, message: {this.exceptionResult.Message}";
            }

            return $"Failed with exception: {this.exceptionResult.GetType()}, message: {this.exceptionResult.Message}";
        }
    }

    public long Time => this.time;

    public TestInfo(
        MethodInfo methodInfo,
        Exception exceptionResult,
        Type expectedException,
        string ignoreMessage,
        long time)
    {
        this.methodInfo = methodInfo;
        this.exceptionResult = exceptionResult;
        this.expectedException = expectedException.SomeNotNull();
        this.ignoreMessage = ignoreMessage.SomeNotNull();
        this.time = time;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"Test method name: {this.Name}");
        stringBuilder.AppendLine($"Status: {this.Status}");
        stringBuilder.AppendLine($"Message: {this.Message}");
        if (this.exceptionResult.StackTrace?.Length > 0)
        {
            stringBuilder.AppendLine($"{this.exceptionResult.StackTrace}");
        }
        stringBuilder.AppendLine($"Time: {this.time}");
        stringBuilder.AppendLine();

        return stringBuilder.ToString();
    }

    private bool IsPassedWithExpectedException()
        => this.expectedException.HasValue &&
           this.expectedException.ValueOrFailure().IsEqual(exceptionResult.GetType());
}