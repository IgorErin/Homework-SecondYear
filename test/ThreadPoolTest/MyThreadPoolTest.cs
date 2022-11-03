namespace ThreadPool;

using Common;
using Exceptions;

/// <summary>
/// <see cref="MyThreadPool"/> Nunit test class.
/// </summary>
public class MyThreadPoolTest
{
    private readonly int processorCount = Environment.ProcessorCount;

    /// <summary>
    /// <see cref="MyThreadPool"/> init threads count test.
    /// </summary>
    /// <param name="threadCount">Init thread count value.</param>
    [TestCase(-1000)]
    [TestCase(-10)]
    [TestCase(0)]
    public void ThreadsCountTest(int threadCount)
    {
        Assert.Throws<MyThreadPoolException>(() =>
        {
            var _ = new MyThreadPool(threadCount);
        });
    }

    /// <summary>
    /// <see cref="MyThreadPool.ShutDown()"/> iterative test.
    /// </summary>
    [Test]
    public void ThreadShutDownTest()
    {
        var expectedResult = new object();

        for (var i = 0; i < 100; i++)
        {
            using var threadPool = new MyThreadPool(this.processorCount);

            var testTask = threadPool.Submit(() => expectedResult);

            threadPool.ShutDown();

            Assert.That(testTask.Result, Is.EqualTo(expectedResult));
        }
    }

    /// <summary>
    /// <see cref="MyThreadPool.ShutDown()"/> iterative test in number of threads.
    /// </summary>
    [Test]
    public void ConcurrentThreadShotDownTest()
    {
        const int arrayLenght = 100;

        var threads = new Thread[arrayLenght];
        var results = new object[arrayLenght];
        var newResult = new object();

        for (var i = 0; i < arrayLenght; i++)
        {
            var localResultIndex = i;

            threads[i] = new Thread(() =>
            {
                using var threadPool = new MyThreadPool(this.processorCount);
                var task = threadPool.Submit(() => newResult);

                threadPool.ShutDown();
                results[localResultIndex] = task.Result;
            });
        }

        threads.StartAndJoinAllThreads();

        Assert.That(results.IsAllTheSameAndNotNull(), Is.True);
    }

    /// <summary>
    /// <see cref="MyThreadPool.Submit{TResult}"/> after <see cref="MyThreadPool.ShutDown()"/> calling test.
    /// </summary>
    [Test]
    public void SubmitAfterShutDownTest()
    {
        var resultObject = new object();

        var threadPool = new MyThreadPool(this.processorCount);
        threadPool.ShutDown();

        Assert.Throws<MyThreadPoolException>(() =>
        {
            var _ = threadPool.Submit(() => resultObject);
        });
    }

    /// <summary>
    /// Calling <see cref="MyTask.MyTask{TResult}.ContinueWith{TNewResult}"/>
    /// after <see cref="MyThreadPool.ShutDown()"/> test.
    /// </summary>
    [Test]
    public void ContinueAfterShutDownTest()
    {
        var resultObject = new object();

        var threadPool = new MyThreadPool(this.processorCount);
        var testTask = threadPool.Submit(() => resultObject);
        threadPool.ShutDown();

        Assert.Throws<MyTaskException>(() =>
        {
            var _ = testTask.ContinueWith(result => result);
        });
    }
}
