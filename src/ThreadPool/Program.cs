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

    //Task.Delay(100).GetAwaiter().GetResult();

    return x;
};

// Work script
var firstTask = threadPool.Submit(myFunc);

firstTask.ContinueWith(myContinuation).Result.Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Ignore();
firstTask.ContinueWith(myContinuation).Result.Ignore();
firstTask.ContinueWith(myContinuation).Ignore();

// // Add to thread pool queue error
// firstTask.ContinueWith(myContinuation).Ignore();
// firstTask.ContinueWith(myContinuation).Ignore();
// firstTask.ContinueWith(myContinuation).Ignore();
// firstTask.ContinueWith(myContinuation).Ignore();
// firstTask.ContinueWith(myContinuation).Ignore();
// var cont = firstTask.ContinueWith(myContinuation);
// threadPool.ShutDown();
// cont.Result.Ignore(); // exception about the error of adding a task to the thread pool


// // Submit error.
// firstTask.ContinueWith(myContinuation).Ignore();
// firstTask.ContinueWith(myContinuation).Ignore();
// firstTask.ContinueWith(myContinuation).Ignore();
// firstTask.ContinueWith(myContinuation).Ignore();
// threadPool.ShutDown();
// firstTask.ContinueWith(myContinuation).Ignore(); // Submit error
