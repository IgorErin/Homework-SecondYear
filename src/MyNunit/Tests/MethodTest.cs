namespace MyNunit.Tests;

using System.Diagnostics;
using System.Reflection;
using System.Text;
using Attributes;
using Exceptions;
using Extensions;
using Optional;
using Optional.Unsafe;

using Methods = IEnumerable<System.Reflection.MethodInfo>;

public class MethodTest : ITest
{
    private static readonly object[] EmptyArgs = Array.Empty<object>();

    private readonly Stopwatch stopwatch = new ();
    private readonly MethodInfo method;

    private readonly object instance;

    private readonly Methods before;
    private readonly Methods after;

    private Option<Type> expected = Option.None<Type>();
    private Option<string> ignore = Option.None<string>();
    private Option<Exception> exception = Option.None<Exception>();
    private Option<Exception> testRunTimeException = Option.None<Exception>();
    private Option<long> time = Option.None<long>();

    private MethodStatus methodStatus = MethodStatus.NotInit;

    public long Time =>
        this.time.Match(
            some: value => value,
            none: () => 0);

    public TestStatus Status => this.ConvertToTestStatus(this.methodStatus);

    public string Message => this.ConvertMethodStatusToString(this.methodStatus);

    public MethodTest(object instance, Methods before, MethodInfo method, Methods after)
    {
        this.instance = instance;

        this.before = before;
        this.method = method;
        this.after = after;
    }

    private enum MethodStatus
    {
        NotInit,
        Compatible,
        Constructor,
        Generic,
        SpecialName,
        IncompatibleParameters,
        IncompatibleReturnType,
        BeforeFailed,
        AfterFailed,
        Passed,
        ReceivedException,
        Ignored,
    }

    public void Run()
    {
        this.methodStatus = IsMethodCompatible(this.method);

        if (this.methodStatus != MethodStatus.Compatible)
        {
            return;
        }

        var attribute = GetTestAttribute(this.method);

        this.expected = attribute.Expected?.Some<Type>() ?? Option.None<Type>();
        this.ignore = attribute.Ignore?.Some<string>() ?? Option.None<string>();

        try
        {
            RunInstanceMethodsWithEmptyArgs(this.instance, this.before);
        }
        catch (Exception beforeException)
        {
            this.testRunTimeException = beforeException.Some();
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

                this.methodStatus = MethodStatus.ReceivedException;
                this.testRunTimeException =
                    testRuntimeException.InnerException?.Some<Exception>() ?? Option.None<Exception>();
            }
            finally
            {
                this.time = this.stopwatch.ElapsedMilliseconds.Some<>();
            }
        }
        else
        {
            this.methodStatus = MethodStatus.Ignored;
        }

        try
        {
            RunInstanceMethodsWithEmptyArgs(this.instance, this.after);
        }
        catch (Exception afterException)
        {
            this.testRunTimeException = afterException.Some();
            this.methodStatus = MethodStatus.AfterFailed;
        }
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(this.Message);

        var state = this.Status;

        if (state == TestStatus.Passed)
        {
            
        }
        else if (state == TestStatus.Ignored)
        {
            if (this.methodStatus == MethodStatus.Ignored)
            {
                stringBuilder.AppendLine($"reason: {this.ignore.ValueOrFailure()}");
            } 
            else if (this.Message == MethodStatus.)
        }
    }

    private static MethodStatus IsMethodCompatible(MethodInfo methodInfo)
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

    private TestStatus ConvertToTestStatus(MethodStatus status)
        => status switch
        {
            MethodStatus.Passed => TestStatus.Passed,
            MethodStatus.NotInit => TestStatus.NotLaunched,
            MethodStatus.ReceivedException => this.GetStateByReceivedException(),
            MethodStatus.Constructor or
                MethodStatus.Generic or
                MethodStatus.SpecialName or
                MethodStatus.IncompatibleParameters or
                MethodStatus.IncompatibleReturnType => TestStatus.Ignored,
            MethodStatus.BeforeFailed or
                MethodStatus.AfterFailed => TestStatus.Failed,
            _ => throw new Exception() //TODO()
        };

    private TestStatus GetStateByReceivedException()
    {
        var receivedException = this.testRunTimeException.ValueOrFailure();

        if (receivedException.GetType().IsEqual(typeof(SuccessException)))
        {
            return TestStatus.Passed;
        }

        if (this.expected.HasValue && this.expected.ValueOrFailure().IsEqual(receivedException.GetType()))
        {
            return TestStatus.Passed;
        }

        return TestStatus.Failed;
    }

    private string ConvertMethodStatusToString(MethodStatus status)
        => status switch
        {
            MethodStatus.Passed => "Test passed",
            MethodStatus.NotInit => "Test was not launched",
            MethodStatus.ReceivedException => this.GetStringByReceivedException(),
            MethodStatus.Constructor => "It looks like you're trying to test the constructor",
            MethodStatus.Generic => "It looks like you're trying to test a generic method",
            MethodStatus.SpecialName => "Methods with a special name cannot be tested",
            MethodStatus.IncompatibleParameters => "It looks like you want to test a method with incompatible parameters",
            MethodStatus.IncompatibleReturnType => "It looks like you want to test a method with an incompatible return type",
            MethodStatus.BeforeFailed => "The method fell during the execution of preparatory actions",
            MethodStatus.AfterFailed => "The test fell during the execution of the final actions",
            _ => "Testing outcome unknown to the system"
        };

    private string GetStringByReceivedException()
    {
        var receivedException = this.testRunTimeException.ValueOrFailure();

        if (receivedException.GetType().IsEqual(typeof(SuccessException)))
        {
            return "The test passed with a successful exception";
        }

        if (this.expected.HasValue && this.expected.ValueOrFailure().IsEqual(receivedException.GetType()))
        {
            return "The test passed with the expected exception";
        }

        return "The test fell with an exception";
    }

    public void Print(ITestPrinter printer)
    {
        return; //TODO()
    }
}