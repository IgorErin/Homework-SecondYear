namespace MyNunit.Visitor;

using Exceptions;
using Extensions;
using Optional;
using Optional.Unsafe;
using Tests.AssemblyTest;
using Tests.MethodTest;
using Tests.TypeTest;

/// <summary>
/// Console test printer <inheritdoc cref="ITestVisitor"/>.
/// </summary>
public class ConsoleTestPrinter : ITestVisitor
{
    /// <summary>
    /// <inheritdoc cref="ITestVisitor.Visit(AssemblyTest)"/>
    /// </summary>
    /// <param name="assembly">Assembly test to visit.</param>
    public void Visit(AssemblyTest assembly)
    {
        Console.WriteLine("Assembly test information");
        Console.WriteLine($"Assembly name: {assembly.FullName}");
    }

    /// <summary>
    /// <inheritdoc cref="ITestVisitor.Visit(TypeTest)"/>.
    /// </summary>
    /// <param name="typeTest">Type test to visit.</param>
    public void Visit(TypeTest typeTest)
    {
        Console.WriteLine($">> Test type name: {typeTest.Name}");
        Console.WriteLine($">> Message: {GetTypeMessage(typeTest.Status, typeTest.Exception)}");
    }

    /// <summary>
    /// <inheritdoc cref="ITestVisitor.Visit(MethodTest)"/>.
    /// </summary>
    /// <param name="methodTest">Method test to visit.</param>
    public void Visit(MethodTest methodTest)
    {
        Console.WriteLine($" => Method name: {methodTest.Name}");
        Console.WriteLine($"   --> Status: {GetTestMethodStatus(methodTest.Status, methodTest.Exception)}");

        methodTest.Time.MatchSome(value => Console.WriteLine($"   --> Time: {value} ms"));

        Console.WriteLine(
            $"   --> Message: {GetTestMethodMessage(methodTest.Status, methodTest.IgnoreMessage, methodTest.Exception)}");
    }

    private static string GetTypeMessage(TypeTestStatus status, Option<Exception> exception)
        => status switch
        {
            TypeTestStatus.NoTestsFound => "No test found",
            TypeTestStatus.AbstractType => "The type cannot be tested because it is abstract",
            TypeTestStatus.IncompatibleConstructorParameters => "The type has incompatible constructor parameters",
            TypeTestStatus.AfterFailed => $"An exception was received when performing post actions, {exception.ValueOrFailure()}",
            TypeTestStatus.BeforeFailed => $"An exception was received when performing pre-actions. {exception.ValueOrFailure()}",
            TypeTestStatus.Passed => "Passed",
            TypeTestStatus.Compatible => throw new TestVisitorException("the type has not been tested yet"),
            _ => throw new TestVisitorException("Type status incompatible")
        };

    private static string GetTestMethodStatus(MethodTestStatus methodTestStatus, Option<Exception> exception)
        => methodTestStatus switch
        {
            MethodTestStatus.Compatible => throw new TestVisitorException("the type has not been tested yet"),
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
            _ => throw new TestVisitorException("method status incompatible")
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
            MethodTestStatus.Compatible => throw new TestVisitorException("the type has not been tested yet"),
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
            _ => throw new TestVisitorException("method status incompatible")
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