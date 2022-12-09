namespace MyNunit.Assembly;

using Optional;
using System.Collections.Concurrent;
using System.Diagnostics;
using TestsInfo;

public class AssemblyTest
{
    private readonly System.Reflection.Assembly assembly;
    private readonly IEnumerable<TypeTest> typeTests;
    private readonly long result;

    public AssemblyTest(System.Reflection.Assembly assembly)
    {
        this.assembly = assembly;
        var resultCollection = new ConcurrentQueue<TypeTest>();
        var assemblyStopWatch = new Stopwatch();

        assemblyStopWatch.Start();

        Parallel.ForEach(this.assembly.ExportedTypes, type =>
        {
            resultCollection.Enqueue(new TypeTest(type));
        });

        assemblyStopWatch.Stop();

        this.result = assemblyStopWatch.ElapsedMilliseconds;
        this.typeTests = resultCollection;
    }
}