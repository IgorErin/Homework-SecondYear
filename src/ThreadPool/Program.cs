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
            Console.WriteLine("LOL");
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
        firstTask = threadPool.Submit(myFunc);

        
        threadPool.ShutDown();
        threadPool.Dispose();
    }
}
