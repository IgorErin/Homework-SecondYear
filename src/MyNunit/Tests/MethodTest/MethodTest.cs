namespace MyNunit.Tests.MethodTest;

using System.Diagnostics;
using System.Reflection;
using Attributes;
using Extensions;
using Optional;
using Optional.Unsafe;
using Printer;
using Methods = IEnumerable<System.Reflection.MethodInfo>;

public class MethodTest
{
    private static readonly object[] EmptyArgs = Array.Empty<object>();

    private readonly Stopwatch stopwatch = new ();
    private readonly MethodInfo method;

    private readonly object instance;

    private readonly Methods before;
    private readonly Methods after;

    private readonly Option<Type> expectedExceptionType;
    private readonly Option<string> ignore;

    private Option<Exception> exception = Option.None<Exception>();
    private Option<long> time = Option.None<long>();

    private MethodTestStatus methodTestStatus;

    public MethodTest(object instance, Methods before, MethodInfo method, Methods after)
    {
        this.instance = instance;

        this.before = before;
        this.method = method;
        this.after = after;

        var attribute = GetTestAttribute(this.method);

        this.expectedExceptionType = attribute.Expected?.Some<Type>() ?? Option.None<Type>();
        this.ignore = attribute.Ignore?.Some<string>() ?? Option.None<string>();

        this.methodTestStatus = GetMethodStatus(this.method);
    }

    public MethodTestStatus Status => this.methodTestStatus;

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

        if (!this.ignore.HasValue)
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
                    testRuntimeException.InnerException?.Some<Exception>() ?? throw new Exception(); //TODO()

                this.methodTestStatus = GetExceptionStatus(this.exception, this.expectedExceptionType);
            }
            finally
            {
                this.time = this.stopwatch.ElapsedMilliseconds.Some<long>(); // TODO()
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

    public void Print(ITestPrinter printer)
        => printer.PrintMethodTest(this.method, this.methodTestStatus, this.time, this.ignore, this.exception);
}