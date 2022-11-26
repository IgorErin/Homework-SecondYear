namespace MyNunit.TestsInfo;

using System.Reflection;
using System.Text;
using Exceptions;
using Extensions;
using Optional;
using Optional.Unsafe;

/// <summary>
/// Class that encapsulates information about test results.
/// </summary>
public class TestInfo
{
    private readonly MethodInfo methodInfo;
    private readonly Option<Exception> exceptionResult;
    private readonly Option<Type> expectedException;
    private readonly Option<string> ignoreMessage;
    private readonly long time;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestInfo"/> class.
    /// </summary>
    /// <param name="methodInfo">Test method information.</param>
    /// <param name="exceptionResult">Optional test exception.</param>
    /// <param name="expectedException">Optional expected test exception.</param>
    /// <param name="ignoreMessage">Optional ignore message.</param>
    /// <param name="time">Test execution time.</param>
    public TestInfo(
        MethodInfo methodInfo,
        Option<Exception> exceptionResult,
        Option<Type> expectedException,
        Option<string> ignoreMessage,
        long time)
    {
        this.methodInfo = methodInfo;
        this.exceptionResult = exceptionResult;
        this.expectedException = expectedException;
        this.ignoreMessage = ignoreMessage;
        this.time = time;
    }

    /// <summary>
    /// Gets the name of test method.
    /// </summary>
    public string Name => this.methodInfo.Name;

    /// <summary>
    /// Gets test execution time.
    /// </summary>
    public long Time => this.time;

    /// <summary>
    /// Gets the result of test.
    /// </summary>
    public (string, long, TestStatus) Result
    {
        get
        {
            if (this.ignoreMessage.HasValue)
            {
                return ($"Ignore, message: {this.ignoreMessage.ValueOrFailure()}", this.time, TestStatus.Ignored);
            }

            if (this.expectedException.HasValue && this.exceptionResult.HasValue)
            {
                var isExpectedExceptionTypeIsEqualToTestException =
                    this.expectedException.ValueOrFailure().IsEqual(this.exceptionResult.ValueOrFailure().GetType());

                if (isExpectedExceptionTypeIsEqualToTestException)
                {
                    return ($"Passed with expected exception, message: {this.exceptionResult.ValueOrFailure().Message}",
                        this.time, TestStatus.Passed);
                }

                return ($"Failed with exception, Expected: {this.expectedException.ValueOrFailure()}, " +
                        $"but received {this.exceptionResult.ValueOrFailure()}", this.time, TestStatus.Failed);
            }

            if (this.expectedException.HasValue)
            {
                return ($"Failed with exception, Expected: {this.expectedException.ValueOrFailure()}, but no exception was received", this.time, TestStatus.Failed);
            }

            if (this.exceptionResult.HasValue)
            {
                var isSuccessExceptionReceived = this.exceptionResult.ValueOrFailure().GetType().IsEqual(typeof(SuccessException));

                if (isSuccessExceptionReceived)
                {
                    return ($"Passed with success exception", this.time, TestStatus.Passed);
                }
            }

            if (this.exceptionResult.GetType().IsEqual(typeof(SuccessException)))
            {
                return ($"Passed", this.time, TestStatus.Passed);
            }

            return ("Failed", this.time, TestStatus.Failed);
        }
    }

    /// <summary>
    /// <see cref="ToString"/> method.
    /// </summary>
    /// <returns>String representation of test results.</returns>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        var (message, _, status) = this.Result;

        stringBuilder.AppendLine($"Test method name: {this.Name}");
        stringBuilder.AppendLine($"Status: {message}");
        stringBuilder.AppendLine($"Message: {status}");

        if (this.exceptionResult.HasValue && this.exceptionResult.ValueOrFailure().StackTrace?.Length > 0)
        {
            stringBuilder.AppendLine($"{this.exceptionResult.ValueOrFailure().StackTrace}");
        }

        stringBuilder.AppendLine($"Time: {this.time}");
        stringBuilder.AppendLine();

        return stringBuilder.ToString();
    }
}
