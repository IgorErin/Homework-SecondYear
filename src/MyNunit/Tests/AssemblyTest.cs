namespace MyNunit.Tests;

using System.Collections.Concurrent;
using System.Diagnostics;

public class AssemblyTest : ITest
{
    private readonly System.Reflection.Assembly assembly;
    private readonly Stopwatch stopwatch;

    private IEnumerable<TypeTest> typeTests;
    private long time;
    private TestStatus status;

    public long Time => this.time;

    public TestStatus Status => this.status;

    public string Message => GetMessage();

    public AssemblyTest(System.Reflection.Assembly assembly)
    {
        this.assembly = assembly;
    }

    public void Run()
    {
        var resultCollection = new ConcurrentQueue<TypeTest>();

        this.stopwatch.Reset();
        this.stopwatch.Start();

        Parallel.ForEach(this.assembly.ExportedTypes, type =>
        {
            var typeTest = new TypeTest(type);

            typeTest.Run();

            resultCollection.Enqueue(typeTest);
        });

        this.stopwatch.Stop();

        this.time = this.stopwatch.ElapsedMilliseconds;
        this.typeTests = resultCollection;
    }

    private string GetMessage()
    {
        return null;
    }

    private void Print(ITestPrinter printer)
    {
        foreach (var typeTest in this.typeTests)
        {
            typeTest.Print(printer);
        }
    }
}