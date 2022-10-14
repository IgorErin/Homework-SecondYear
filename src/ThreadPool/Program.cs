namespace ThreadPool;

class PoolMain
{
    /// <summary>
    /// <see cref="MyThreadPool"/> use case.
    /// </summary>
    public static void Main()
    {
        using var threadPool = new MyThreadPool(4);

        var myFunc = () =>
        {
            Console.WriteLine("lol");
            return 2 * 2;
        };

        var myContinuation = (int x) =>
        {
            Console.WriteLine($"Result = {x}");
            return x;
        };

        var firstTask = threadPool.Submit(myFunc);
        
        firstTask.ContinueWith(myContinuation);


        for (var i = 0; i < 100000; i++)
        {
            Task.Delay(10000000);
        }
        Task.Delay(10000000);
    }
}
