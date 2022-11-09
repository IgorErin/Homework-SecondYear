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

        var _ = Hash.getSum(Directory.GetCurrentDirectory()).GetAwaiter().GetResult();

        watch.Stop();

        Console.WriteLine($"result = {watch.ElapsedMilliseconds}");

        watch.Reset();
        watch.Start();

        _ = ParallelHash.getSum(Directory.GetCurrentDirectory()).GetAwaiter().GetResult();

        watch.Stop();

        Console.WriteLine($"parallel result = {watch.ElapsedMilliseconds}");
    }
}