namespace ThreadPool;

class PoolMain
{
    /// <summary>
    /// <see cref="MyThreadPool"/> use case.
    /// </summary>
    public static void Main()
    {
        Console.WriteLine($"main thread: {Environment.CurrentManagedThreadId}");
        Console.WriteLine($"main thread: {Environment.CurrentManagedThreadId}");
        using var threadPool = new MyThreadPool(4);

        var myFunc = () =>
        {
            Console.WriteLine("LOL");
            Task.Delay(10000);
            return 2 * 2;
        };

        var myContinuation = (int x) =>
        {
            Console.WriteLine($"Result = {x}");
            return x;
        };

        var firstTask = threadPool.Submit(myFunc);
        firstTask = threadPool.Submit(myFunc);
        firstTask = threadPool.Submit(myFunc);
        firstTask = threadPool.Submit(myFunc);
        firstTask = threadPool.Submit(myFunc);
        firstTask = threadPool.Submit(myFunc);
        firstTask = threadPool.Submit(myFunc);
        firstTask = threadPool.Submit(myFunc);
        firstTask = threadPool.Submit(myFunc);
        firstTask = threadPool.Submit(myFunc);
        firstTask = threadPool.Submit(myFunc);
        
        var result = firstTask.Result;
        var continuation = firstTask.ContinueWith(myContinuation);


        Task.Delay(10000);
        threadPool.ShutDown();
        threadPool.Dispose();
    }
}
