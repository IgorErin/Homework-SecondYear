namespace MyNunit.Tests.MethodTest;

using System.Diagnostics;
using System.Reflection;
using Attributes;
using Extensions;
using Optional;
using Optional.Unsafe;
using Visitor;
using Methods = IEnumerable<System.Reflection.MethodInfo>;

/// <summary>
/// Method test.
/// </summary>
public class MethodTest
{
    private static readonly object[] EmptyArgs = Array.Empty<object>();

    private readonly Stopwatch stopwatch = new ();
    private readonly MethodInfo method;

    private readonly object instance;

    private readonly Methods before;
    private readonly Methods after;

    private readonly Option<Type> expectedExceptionType;
    private readonly Option<string> ignoreMessage;

    private Option<Exception> exception = Option.None<Exception>();
    private Option<long> time = Option.None<long>();

    private MethodTestStatus methodTestStatus;

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodTest"/> class.
    /// </summary>
    /// <param name="instance">Instance whose method is being tested.</param>
    /// <param name="before">Methods performed before testing.</param>
    /// <param name="method">Method to test.</param>
    /// <param name="after">Methods to perform after testing.</param>
    public MethodTest(object instance, Methods before, MethodInfo method, Methods after)
    {
        this.instance = instance;

        this.before = before;
        this.method = method;
        this.after = after;

        var attribute = GetTestAttribute(this.method);

        this.expectedExceptionType = attribute.Expected?.Some<Type>() ?? Option.None<Type>();
        this.ignoreMessage = attribute.Ignore?.Some<string>() ?? Option.None<string>();

        this.methodTestStatus = GetMethodStatus(this.method);
    }

    /// <summary>
    /// Gets test status.
    /// </summary>
    public MethodTestStatus Status => this.methodTestStatus;

    /// <summary>
    /// Gets optional test run time exception.
    /// </summary>
    public Option<Exception> Exception => this.exception;

    /// <summary>
    /// Gets optional <see cref="TestAttribute"/> ignore message.
    /// </summary>
    public Option<string> IgnoreMessage => this.ignoreMessage;

    /// <summary>
    /// Gets optional test time of execution.
    /// </summary>
    public Option<long> Time => this.time;

    /// <summary>
    /// Gets test name.
    /// </summary>
    public string Name => this.method.Name;

    /// <summary>
    /// Run test.
    /// </summary>
    public void Run()
    {
        if (this.methodTestStatus != MethodTestStatus.Compatible)
        {
            return;
        }

        try
        {
            RunInstanceMethodsWithEmptyArgs(this.instance, this.before);
        }
        catch (Exception beforeException)
        {
            this.exception = beforeException.Some();
            this.methodTestStatus = MethodTestStatus.BeforeFailed;

            return;
        }

        if (!this.ignoreMessage.HasValue)
        {
            try
            {
                this.stopwatch.Reset();
                this.stopwatch.Start();

                RunInstanceMethodWithEmptyArgs(this.instance, this.method);

                this.stopwatch.Stop();
                this.methodTestStatus = MethodTestStatus.Passed;
            }
            catch (Exception testRuntimeException)
            {
                this.stopwatch.Stop();

                this.exception =
                    testRuntimeException.InnerException?.Some<Exception>() ??
                        throw new NullReferenceException("Null inner exception");

                this.methodTestStatus = GetExceptionStatus(this.exception, this.expectedExceptionType);
            }
            finally
            {
                this.time = this.stopwatch.ElapsedMilliseconds.Some<long>();
            }
        }
        else
        {
            this.methodTestStatus = MethodTestStatus.IgnoredWithMessage;
        }

        try
        {
            RunInstanceMethodsWithEmptyArgs(this.instance, this.after);
        }
        catch (Exception afterException)
        {
            this.exception = afterException.Some();
            this.methodTestStatus = MethodTestStatus.AfterFailed;
        }
    }

    /// <summary>
    /// Accept <see cref="ITestVisitor"/>.
    /// </summary>
    /// <param name="visitor">Visitor to accept.</param>
    public void Accept(ITestVisitor visitor)
        => visitor.Visit(this);

    private static MethodTestStatus GetMethodStatus(MethodInfo methodInfo)
    {
        if (methodInfo.IsConstructor)
        {
            return MethodTestStatus.Constructor;
        }

        if (methodInfo.IsGenericMethodDefinition)
        {
            return MethodTestStatus.Generic;
        }

        if (methodInfo.IsSpecialName)
        {
            return MethodTestStatus.SpecialName;
        }

        if (methodInfo.GetParameters().Length != 0)
        {
            return MethodTestStatus.IncompatibleParameters;
        }

        if (methodInfo.ReturnType != typeof(void))
        {
            return MethodTestStatus.IncompatibleReturnType;
        }

        return MethodTestStatus.Compatible;
    }

    private static MethodTestStatus GetExceptionStatus(Option<Exception> receivedException, Option<Type> expectedType)
    {
        var received = receivedException.ValueOrFailure("TODO()");

        return expectedType.Match(
            some: value =>
            {
                if (received.GetType().IsEqual(value))
                {
                    return MethodTestStatus.ReceivedExpectedException;
                }

                return MethodTestStatus.ReceivedUnexpectedException;
            },
            none: () => MethodTestStatus.ReceivedUnexpectedException);
    }

    private static TestAttribute GetTestAttribute(MethodInfo type)
        => (TestAttribute)(Attribute.GetCustomAttribute(type, typeof(TestAttribute))
                           ?? throw new NullReferenceException("Empty method attributes"));

    private static void RunInstanceMethodsWithEmptyArgs(object instance, Methods methods)
    {
        foreach (var afterMethod in methods)
        {
            RunInstanceMethodWithEmptyArgs(instance, afterMethod);
        }
    }

    private static void RunInstanceMethodWithEmptyArgs(object type, MethodInfo methodInfo)
        => methodInfo.Invoke(type, EmptyArgs);
}