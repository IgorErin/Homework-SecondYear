namespace MyNunit;

using System.Reflection;
using Tests.AssemblyTest;
using Visitor;

/// <summary>
/// Class for testing assemblies.
/// </summary>
public class MyNunit
{
    private readonly IEnumerable<AssemblyTest> tests;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assemblies"></param>
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

    public void Visit(ITestVisitor visitor)
    {
        foreach (var test in this.tests)
        {
            test.Accept(visitor);
        }
    }
}
