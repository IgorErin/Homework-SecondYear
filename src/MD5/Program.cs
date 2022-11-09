// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using MD5;

Console.WriteLine("Hello, World!");

class MDMain
{
    public static void Main()
    {
        var watch = new Stopwatch();

        watch.Start();

        var _ = Hash.getSum("D:\\Projects\\Practic").GetAwaiter().GetResult();

        watch.Stop();

        Console.Write($"result = {watch.ElapsedMilliseconds}");

        watch.Reset();
        watch.Start();

        _ = ParallelHash.getSum("D:\\Projects\\Practic").GetAwaiter().GetResult();

        watch.Stop();

        Console.Write($"parallel result = {watch.ElapsedMilliseconds}");
    }
}