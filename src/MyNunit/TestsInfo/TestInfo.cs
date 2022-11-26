namespace MyNunit.TestsInfo;

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

    public long Time => this.time;

    public (string, long, TestStatus) Result
    {
        get
        {
            if (this.ignoreMessage.HasValue)
            {
                return ($"Ignore, message: {ignoreMessage.ValueOrFailure()}", this.time, TestStatus.Ignored);
            }

            if (this.expectedException.HasValue && this.expectedException.ValueOrFailure().IsEqual(exceptionResult.GetType()))
            {
                return ($"Passed with expected exception, message: {this.exceptionResult.Message}", this.time, TestStatus.Passed);
            }

            if (this.expectedException.HasValue)
            {
                return ($"Failed with exception, message {this.exceptionResult.Message}", this.time, TestStatus.Failed);
            }

            if (this.exceptionResult.GetType().IsEqual(typeof(SuccessException)))
            {
                return ($"Passed", this.time, TestStatus.Passed);
            }

            return ("Failed", this.time, TestStatus.Failed);
        }
    }

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

        var (message, _, status) = this.Result;

        stringBuilder.AppendLine($"Test method name: {this.Name}");
        stringBuilder.AppendLine($"Status: {message}");
        stringBuilder.AppendLine($"Message: {status}");
        if (this.exceptionResult.StackTrace?.Length > 0)
        {
            stringBuilder.AppendLine($"{this.exceptionResult.StackTrace}");
        }
        stringBuilder.AppendLine($"Time: {this.time}");
        stringBuilder.AppendLine();

        return stringBuilder.ToString();
    }
}
