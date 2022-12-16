using ThreadPool;
using ThreadPool.Extensions;

Console.WriteLine($"main thread: {Environment.CurrentManagedThreadId}");
using var threadPool = new MyThreadPool(4);

var myFunc = () =>
{
    Task.Delay(100).GetAwaiter().GetResult();
    Console.WriteLine($"FirstTask thread = {Environment.CurrentManagedThreadId}");

    return 2 * 2;
};

var myContinuation = (int x) =>
{
    Task.Delay(100).GetAwaiter().GetResult();
    Console.WriteLine($"Continuation thread = {Environment.CurrentManagedThreadId}");
    Console.WriteLine($"Result = {x}");

    return x;
};

var firstTask = threadPool.Submit(myFunc);

// // Number of continuations.
// firstTask.ContinueWith(myContinuation).Result.Ignore();
// firstTask.ContinueWith(myContinuation).Ignore();
// firstTask.ContinueWith(myContinuation).Ignore();
// firstTask.ContinueWith(myContinuation).Result.Ignore();
// firstTask.ContinueWith(myContinuation).Ignore();

// Computation in user thread.
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Ignore(); // the continuations above will not have time to get on the threadpool
var cont = firstTask.ContinueWith(myContinuation);
threadPool.ShutDown();
cont.Result.Ignore(); // no exception thrown --- the task is calculated in the user thread

// Submit error.
// firstTask.ContinueWith(myContinuation).Ignore();
// firstTask.ContinueWith(myContinuation).Ignore();
// firstTask.ContinueWith(myContinuation).Ignore();
// firstTask.ContinueWith(myContinuation).Ignore();
// threadPool.ShutDown();
// firstTask.ContinueWith(myContinuation).Ignore(); // Submit error
