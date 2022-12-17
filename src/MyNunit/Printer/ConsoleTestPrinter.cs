namespace MyNunit.Printer;

using System.Reflection;
using System.Text;
using Optional;
using Tests.MethodTest;
using Tests.TypeTest;

public class ConsoleTestPrinter : ITestPrinter
{
    public void PrintAssemblyTest(Assembly assembly)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine("Assembly Test");
        stringBuilder.AppendLine($"Assembly name: {assembly.FullName}");

        Console.WriteLine(stringBuilder.ToString());
    }

    public void PrintTypeTest(TypeInfo typeInfo, TypeTestStatus status, Option<Exception> exception)
    {
        Console.WriteLine($"Test type name: {typeInfo.Name}");
    }

    public void PrintMethodTest(
        MethodInfo methodInfo,
        MethodStatus methodStatus,
        Option<string> ignoreMessage,
        Option<Exception> exception)
    {
        Console.WriteLine($"Method name: {methodInfo.Name}");
        Console.WriteLine($"Test status: {methodStatus}");
    }
}