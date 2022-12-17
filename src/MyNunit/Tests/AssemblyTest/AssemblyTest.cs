namespace MyNunit.Tests.AssemblyTest;

using System.Collections.Concurrent;
using Optional;
using Optional.Unsafe;
using Printer;
using TypeTest;

public class AssemblyTest
{
    private readonly System.Reflection.Assembly assembly;

    private Option<IEnumerable<TypeTest>> typeTests;

    public AssemblyTest(System.Reflection.Assembly assembly)
    {
        this.assembly = assembly;
    }

    public void Run()
    {
        var resultCollection = new ConcurrentQueue<TypeTest>();

        Parallel.ForEach(this.assembly.ExportedTypes, type =>
        {
            var typeTest = new TypeTest(type);

            typeTest.Run();

            resultCollection.Enqueue(typeTest);
        });

        this.typeTests = resultCollection.SomeNotNull<>();
    }

    public void Print(ITestPrinter printer)
    {
        foreach (var typeTest in this.typeTests.ValueOrFailure())
        {
            typeTest.Print(printer);
        }
    }
}