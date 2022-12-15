using ThreadPool;
using ThreadPool.Extensions;

Console.WriteLine($"main thread: {Environment.CurrentManagedThreadId}");
using var threadPool = new MyThreadPool(4);

var myFunc = () =>
{
    Console.WriteLine($"FirstTask thread = {Environment.CurrentManagedThreadId}");

    Task.Delay(100).GetAwaiter().GetResult();

    return 2 * 2;
};

var myContinuation = (int x) =>
{
    Console.WriteLine($"Continuation thread = {Environment.CurrentManagedThreadId}");
    Console.WriteLine($"Result = {x}");

    Task.Delay(100).GetAwaiter().GetResult();

    return x;
};

var firstTask = threadPool.Submit(myFunc);

firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Result.Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Result.Ignore();


