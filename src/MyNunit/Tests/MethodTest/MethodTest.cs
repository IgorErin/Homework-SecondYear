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

    private MethodStatus methodStatus;

    public MethodTest(object instance, Methods before, MethodInfo method, Methods after)
    {
        this.instance = instance;

        this.before = before;
        this.method = method;
        this.after = after;

        var attribute = GetTestAttribute(this.method);

        this.expectedExceptionType = attribute.Expected?.Some<Type>() ?? Option.None<Type>();
        this.ignore = attribute.Ignore?.Some<string>() ?? Option.None<string>();

        this.methodStatus = GetMethodStatus(this.method);
    }

    public void Run()
    {
        if (this.methodStatus != MethodStatus.Compatible)
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
            this.methodStatus = MethodStatus.BeforeFailed;

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
                this.methodStatus = MethodStatus.Passed;
            }
            catch (Exception testRuntimeException)
            {
                this.stopwatch.Stop();

                this.exception =
                    testRuntimeException.InnerException?.Some<Exception>() ?? throw new Exception(); //TODO()

                this.methodStatus = GetExceptionStatus(this.exception, this.expectedExceptionType);
            }
            finally
            {
                this.time = this.stopwatch.ElapsedMilliseconds.Some<>(); // TODO()
            }
        }
        else
        {
            this.methodStatus = MethodStatus.IgnoredWithMessage;
        }

        try
        {
            RunInstanceMethodsWithEmptyArgs(this.instance, this.after);
        }
        catch (Exception afterException)
        {
            this.exception = afterException.Some();
            this.methodStatus = MethodStatus.AfterFailed;
        }
    }

    private static MethodStatus GetMethodStatus(MethodInfo methodInfo)
    {
        if (methodInfo.IsConstructor)
        {
            return MethodStatus.Constructor;
        }

        if (methodInfo.IsGenericMethodDefinition)
        {
            return MethodStatus.Generic;
        }

        if (methodInfo.IsSpecialName)
        {
            return MethodStatus.SpecialName;
        }

        if (methodInfo.GetParameters().Length != 0)
        {
            return MethodStatus.IncompatibleParameters;
        }

        if (methodInfo.ReturnType == typeof(void))
        {
            return MethodStatus.IncompatibleReturnType;
        }

        return MethodStatus.Compatible;
    }

    private static MethodStatus GetExceptionStatus(Option<Exception> receivedException, Option<Type> expectedType)
    {
        var received = receivedException.ValueOrFailure("TODO()");

        return expectedType.Match(
            some: value =>
            {
                if (received.GetType().IsEqual(value))
                {
                    return MethodStatus.ReceivedExpectedException;
                }

                return MethodStatus.ReceivedUnexpectedException;
            },
            none: () => MethodStatus.ReceivedException);
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
    {
        return; //TODO()
    }
}