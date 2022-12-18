namespace MyNunit.Tests.AssemblyTest;

using System.Collections.Concurrent;
using System.Reflection;
using Printer;
using TypeTest;

public class AssemblyTest
{
    private readonly Assembly assembly;

    private readonly ConcurrentQueue<TypeTest> typeTests = new ();

    public AssemblyTest(Assembly assembly)
    {
        this.assembly = assembly;
    }

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

    public void Print(ITestPrinter printer)
    {
        printer.PrintAssemblyTest(this.assembly);

        foreach (var typeTest in this.typeTests)
        {
            typeTest.Print(printer);
        }
    }
}