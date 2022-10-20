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
            return 2 * 2;
        };

        var myContinuation = (int x) =>
        {
            Console.WriteLine($"core = {Environment.CurrentManagedThreadId}");
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
        
        var continuation = firstTask.ContinueWith(myContinuation);

        var _ = continuation.Result;
        threadPool.ShutDown();
        
        threadPool.Dispose();
    }
}
