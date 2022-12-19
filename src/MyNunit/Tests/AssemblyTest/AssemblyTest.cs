namespace MyNunit.Tests.AssemblyTest;

using System.Collections.Concurrent;
using System.Reflection;
using TypeTest;
using Visitor;

/// <summary>
/// <see cref="Assembly"/> test class.
/// </summary>
public class AssemblyTest
{
    private readonly Assembly assembly;

    private readonly ConcurrentQueue<TypeTest> typeTests = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyTest"/> class.
    /// </summary>
    /// <param name="assembly">Assembly to test.</param>
    public AssemblyTest(Assembly assembly)
    {
        this.assembly = assembly;
    }

    /// <summary>
    /// Gets full assembly name.
    /// </summary>
    public string FullName => this.assembly.FullName ?? "Name unknown";

    /// <summary>
    /// Run assembly test.
    /// </summary>
    public void Run()
    {
        this.typeTests.Clear();

        Parallel.ForEach(this.assembly.ExportedTypes, type =>
        {
            var typeTest = new TypeTest(type);

            typeTest.Run();

            this.typeTests.Enqueue(typeTest);
        });
    }

    /// <summary>
    /// Accept <see cref="ITestVisitor"/>.
    /// </summary>
    /// <param name="visitor">Object that implement <see cref="ITestVisitor"/>.</param>
    public void Accept(ITestVisitor visitor)
    {
        visitor.Visit(this);

        foreach (var typeTest in this.typeTests)
        {
            typeTest.Accept(visitor);
        }
    }
}