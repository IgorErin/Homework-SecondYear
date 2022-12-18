namespace MyNunit.Printer;

using System.Reflection;
using Exceptions;
using Extensions;
using Optional;
using Optional.Unsafe;
using Tests.MethodTest;
using Tests.TypeTest;

public class ConsoleTestPrinter : ITestPrinter
{
    public void PrintAssemblyTest(Assembly assembly)
    {
        Console.WriteLine("Assembly test information");
        Console.WriteLine($"Assembly name: {assembly.FullName}");
    }

    public void PrintTypeTest(TypeInfo typeInfo, TypeTestStatus status, Option<Exception> exception)
    {
        Console.WriteLine($">> Test type name: {typeInfo.Name}");
        Console.WriteLine($">> Status: {GetTypeMessage(status, exception)}");
    }

    public void PrintMethodTest(
        MethodInfo methodInfo,
        MethodTestStatus methodTestStatus,
        Option<long> time,
        Option<string> ignoreMessage,
        Option<Exception> exception)
    {
        Console.WriteLine($" => Method name: {methodInfo.Name}");
        Console.WriteLine($"   --> Status: {GetTestMethodStatus(methodTestStatus, exception)}");

        time.MatchSome(value => Console.WriteLine($"   --> Time: {value} ms"));

        Console.WriteLine($"   --> Message: {GetTestMethodMessage(methodTestStatus, ignoreMessage, exception)}");
    }
    
    private static string GetTypeMessage(TypeTestStatus status, Option<Exception> exception)
        => status switch
        {
            TypeTestStatus.NoTestsFound => "No test found",
            TypeTestStatus.AbstractType => "The type cannot be tested because it is abstract",
            TypeTestStatus.IncompatibleConstructorParameters => "The type has incompatible constructor parameters",
            TypeTestStatus.AfterFailed => "An exception was received when performing post actions",
            TypeTestStatus.BeforeFailed => "An exception was received when performing pre-actions",
            TypeTestStatus.Passed => "Passed",
            TypeTestStatus.Compatible => throw new Exception(), //TODO()
            _ => throw new Exception() // TODO()
        };

    private static string GetTestMethodStatus(MethodTestStatus methodTestStatus, Option<Exception> exception)
        => methodTestStatus switch
        {
            MethodTestStatus.Compatible => throw new Exception(), // TODO()
            MethodTestStatus.Constructor or
                MethodTestStatus.Generic or
                MethodTestStatus.IncompatibleParameters or
                MethodTestStatus.IncompatibleReturnType or
                MethodTestStatus.SpecialName => "Incompatible",
            MethodTestStatus.AfterFailed or
            MethodTestStatus.BeforeFailed => "Failed",
            MethodTestStatus.ReceivedUnexpectedException => GetMethodTestStatusByException(exception.ValueOrFailure()),
            MethodTestStatus.IgnoredWithMessage or
                MethodTestStatus.ReceivedExpectedException or
                MethodTestStatus.Passed => "Passed",
            _ => throw new Exception() //TODO()
        };

    private static string GetMethodTestStatusByException(Exception exception)
    {
        if (exception.GetType().IsEqual(typeof(SuccessException)))
        {
            return "Passed";
        }

        if (exception.GetType().IsEqual(typeof(SuccessException)))
        {
            return "Failed";
        }

        return "Failed";
    }

    private static string GetTestMethodMessage(
        MethodTestStatus methodTestStatus,
        Option<string> ignoreMessage,
        Option<Exception> exception)
        => methodTestStatus switch
        {
            MethodTestStatus.Compatible => throw new Exception(), // TODO()
            MethodTestStatus.Constructor => "This method is a constructor --- cannot be tested",
            MethodTestStatus.Generic => "Generalized method --- incompatible for testing",
            MethodTestStatus.IncompatibleParameters or
                MethodTestStatus.IncompatibleReturnType => "The method has an incompatible signature",
            MethodTestStatus.SpecialName
                => "The method has a special name --- not compatible for testing",
            MethodTestStatus.AfterFailed
                => $"An exception was received when performing post actions: {exception.ValueOrFailure()}",
            MethodTestStatus.BeforeFailed
                => $"An exception was received when performing pre-actions: {exception.ValueOrFailure()}",
            MethodTestStatus.IgnoredWithMessage =>
                $"The test is specified as ignored, the reason: {ignoreMessage.ValueOrFailure()}",
            MethodTestStatus.ReceivedExpectedException =>
                $"An expected exception was received: {exception.ValueOrFailure().GetType()}",
            MethodTestStatus.ReceivedUnexpectedException => GetMethodTestMessageByException(exception.ValueOrFailure()),
            MethodTestStatus.Passed => "Test passed",
            _ => throw new Exception() //TODO()
        };

    private static string GetMethodTestMessageByException(Exception exception)
    {
        if (exception.GetType().IsEqual(typeof(SuccessException)))
        {
            return "Passed with success exception";
        }

        if (exception.GetType().IsEqual(typeof(FailException)))
        {
            return "Failed with fail exception";
        }

        return $"Failed with unexpected exception: {exception}";
    }
}