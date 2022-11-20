namespace MyNunit;

using System.Reflection;
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

            if (this.expectedException.HasValue
                && this.exceptionResult.GetType() == this.expectedException.GetType())
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
                return $"Ignore, {ignoreMessage.ValueOrFailure()}";
            }

            if (this.expectedException.HasValue)
            {
                if (this.expectedException.ValueOrFailure().GetType() == exceptionResult.GetType()) //TODO()
                {
                    return $"Passed with expected exception, message: {this.exceptionResult.Message}";
                }

                return
                    $"Failed with unexpected exception: {this.exceptionResult.GetType()}," +
                    $" message = {this.exceptionResult.Message}";
            }

            return $"Failed with exception: {this.exceptionResult.GetType()}, message = {this.exceptionResult.Message}";
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
}