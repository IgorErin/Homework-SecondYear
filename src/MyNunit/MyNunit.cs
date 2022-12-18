namespace MyNunit;

using System.Reflection;
using Printer;
using Tests.AssemblyTest;

/// <summary>
/// Class for testing assemblies.
/// </summary>
public class MyNunit
{
    private readonly IEnumerable<AssemblyTest> tests;

    public MyNunit(IEnumerable<Assembly> assemblies)
    {
        var testList = new List<AssemblyTest>();

        foreach (var assembly in assemblies)
        {
            testList.Add(new AssemblyTest(assembly));
        }

        this.tests = testList;
    }

    public void Run()
    {
        foreach (var test in this.tests)
        {
            test.Run();
        }
    }

    public void Print(ITestPrinter printer)
    {
        foreach (var test in this.tests)
        {
            test.Print(printer);
        }
    }
}
