namespace MyNunit;

using Optional;
using Optional.Unsafe;

public class TestInfo
{
    private readonly string name;
    private readonly Exception exceptionResult;
    private readonly Option<Type> expectedException;
    private readonly Option<string> ignoreMessage;
    private readonly long time;

    public string Name => this.name;

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
        string name,
        Exception exceptionResult,
        Type expectedException,
        string ignoreReason,
        long time)
    {
        this.name = name;
        this.exceptionResult = exceptionResult;
        this.expectedException = expectedException.SomeNotNull();
        this.ignoreMessage = ignoreReason.SomeNotNull();
        this.time = time;
    }
}