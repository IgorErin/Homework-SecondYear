namespace MyNunit.Printer;

using System.Reflection;
using Optional;
using Tests.MethodTest;
using Tests.TypeTest;

public interface ITestPrinter
{
    public void PrintAssemblyTest(Assembly assembly);

    public void PrintTypeTest(TypeInfo typeInfo, TypeTestStatus status, Option<Exception> exception);

    public void PrintMethodTest(
        MethodInfo methodInfo,
        MethodStatus methodStatus,
        Option<string> ignoreMessage,
        Option<Exception> exception);
}