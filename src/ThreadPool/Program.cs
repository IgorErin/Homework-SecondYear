using ThreadPool;
using ThreadPool.Extensions;

Console.WriteLine($"main thread: {Environment.CurrentManagedThreadId}");
using var threadPool = new MyThreadPool(4);

var myFunc = () =>
{
    Console.WriteLine($"FirstTask core = {Environment.CurrentManagedThreadId}");

    return 2 * 2;
};

var myContinuation = (int x) =>
{
    Console.WriteLine($"Continuation core = {Environment.CurrentManagedThreadId}");
    Console.WriteLine($"Result = {x}");

    return x;
};

var firstTask = threadPool.Submit(myFunc);

firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Result.Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Result.Ignore();


