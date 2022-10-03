using ThreadPool;

class PoolMain
{
    /// <summary>
    /// <see cref="MyThreadPool"/> use case.
    /// </summary>
    public static void Main()
    {
        using var threadPool = new MyThreadPool(4);

        var myFunc = () => 2 * 2;
        var myContinuation = (int x) =>
        {
            Console.WriteLine($"Result = {x}");
            return x;
        };

        var firstTask = threadPool.Submit(myFunc);
        firstTask.ContinueWith(myContinuation);

        threadPool.ShutDown();
    }
}
