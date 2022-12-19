using ThreadPool;
using ThreadPool.Extensions;

Console.WriteLine($"main thread: {Environment.CurrentManagedThreadId}");
using var threadPool = new MyThreadPool(4);

var myFunc = () =>
{
    Console.WriteLine($"FirstTask thread = {Environment.CurrentManagedThreadId}");

    return 2 * 2;
};

var myContinuation = (int x) =>
{
    Console.WriteLine($"Continuation thread = {Environment.CurrentManagedThreadId}");
    Console.WriteLine($"Result = {x}");

    return x;
};

var firstTask = threadPool.Submit(myFunc);

// Number of continuations.;
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
Task.Delay(1000).GetAwaiter().GetResult();
var cont = firstTask.ContinueWith(myContinuation);

threadPool.ShutDown();

cont.Result.Ignore();

// // Submit error.
// firstTask.ContinueWith(myContinuation).Ignore();
// firstTask.ContinueWith(myContinuation).Ignore();
// firstTask.ContinueWith(myContinuation).Ignore();
// firstTask.ContinueWith(myContinuation).Ignore();
// threadPool.ShutDown();
// firstTask.ContinueWith(myContinuation).Ignore(); // Submit error
