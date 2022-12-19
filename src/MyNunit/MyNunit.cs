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
    /// Initializes a new instance of the <see cref="MyNunit"/> class.
    /// </summary>
    /// <param name="assemblies">Assemblies to test.</param>
    public MyNunit(IEnumerable<Assembly> assemblies)
    {
        var testList = new List<AssemblyTest>();

        foreach (var assembly in assemblies)
        {
            testList.Add(new AssemblyTest(assembly));
        }

        this.tests = testList;
    }

    /// <summary>
    /// Run tests.
    /// </summary>
    public void Run()
    {
        foreach (var test in this.tests)
        {
            test.Run();
        }
    }

    /// <summary>
    /// Visit all of <see cref="AssemblyTest"/>.
    /// </summary>
    /// <param name="visitor">Visitor for visit.</param>
    public void Visit(ITestVisitor visitor)
    {
        foreach (var test in this.tests)
        {
            test.Accept(visitor);
        }
    }
}
