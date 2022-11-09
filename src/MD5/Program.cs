using System.Diagnostics;
using MD5;

class MDMain
{
    private const int count = 10;

    public static void Main()
    {
        var watch = new Stopwatch();

        var timeArray = new long[count];

        for (var i = 0; i < count; i++)
        {
            watch.Start();

            var _ = Hash.getSum(Directory.GetCurrentDirectory()).GetAwaiter().GetResult();

            watch.Stop();

            timeArray[i] = watch.ElapsedMilliseconds;

            watch.Reset();
        }

        Console.WriteLine($"sequential result = {Enumerable.Average(timeArray)}");

        timeArray = new long[count];

        for (var i = 0; i < count; i++)
        {
            watch.Start();

            var _ = Hash.getSum(Directory.GetCurrentDirectory()).GetAwaiter().GetResult();

            watch.Stop();

            timeArray[i] = watch.ElapsedMilliseconds;

            watch.Reset();
        }

        Console.WriteLine($"parallel result = {Enumerable.Average(timeArray)}");
    }
}