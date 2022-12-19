namespace MyNunit.Tests.AssemblyTest;

using System.Collections.Concurrent;
using System.Reflection;
using TypeTest;
using Visitor;

public class AssemblyTest
{
    private readonly Assembly assembly;

    private readonly ConcurrentQueue<TypeTest> typeTests = new ();

    public AssemblyTest(Assembly assembly)
    {
        this.assembly = assembly;
    }

    public string FullName => this.assembly.FullName ?? "Name unknown";

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

    public void Accept(ITestVisitor visitor)
    {
        visitor.Visit(this);

        foreach (var typeTest in this.typeTests)
        {
            typeTest.Accept(visitor);
        }
    }
}