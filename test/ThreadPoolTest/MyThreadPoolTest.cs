using ThreadPool.Common;
using ThreadPool.Exceptions;

namespace ThreadPool;

public class MyThreadPoolTest
{
    private readonly int _processorCount = Environment.ProcessorCount;
    
    [TestCase(-1000)]
    [TestCase(-10)]
    [TestCase(0)]
    public void ThreadsCountTest(int threadCount)
    {
        try
        {
            using var threadPool = new MyThreadPool(threadCount);
            
            Assert.Fail();
        }
        catch (MyThreadPoolException e)
        {
            Assert.Pass();
        }
    }

    [Test]
    public void ThreadShutDownTest()
    {
        var expectedResult = new object();
        
        for (var i = 0; i < 100; i++)
        {
            using var threadPool = new MyThreadPool(_processorCount);

            var testTask = threadPool.Submit(() => expectedResult);
            
            threadPool.ShutDown();

            var taskResult = testTask.Result;
            
            Assert.That(taskResult, Is.EqualTo(expectedResult));
        }
    }

    [Test]
    public void ConcurrentThreadShotDownTest()
    {
        var threads = new Thread[_processorCount];
        var results = new object[_processorCount];
        var newResult = new object();

        for (var i = 0; i < _processorCount; i++)
        {
            var localResultIndex = i;
            
            threads[i] = new Thread(() =>
            {
                using var threadPool = new MyThreadPool(_processorCount);
                var task = threadPool.Submit(() => newResult);
                
                threadPool.ShutDown();
                results[localResultIndex] = task.Result;
            });
        }
        
        threads.StartAndJoinAllThreads();
        
        Assert.True(results.IsAllEqualAndNotNull());
    }

    [Test]
    public void SubmitAfterShutDownTest()
    {
        var resultObject = new object();
        
        var threadPool = new MyThreadPool(_processorCount);
        threadPool.ShutDown();

        try
        {
            var testTask = threadPool.Submit(() => resultObject);
            
            Assert.Fail();
        }
        catch (MyThreadPoolException e)
        {
            Assert.Pass();
        }
    }
    
    [Test]
    public void ContinueAfterShutDownTest()
    {
        var resultObject = new object();
        
        var threadPool = new MyThreadPool(_processorCount);
        var testTask = threadPool.Submit(() => resultObject);
        threadPool.ShutDown();


        try
        {
            var continueTask = testTask.ContinueWith(result =>
            {
                Console.Write(result);
                return 1;
            });
            
            Assert.Fail();
        }
        catch (MyThreadPoolException e)
        {
            Assert.Pass();
        }
    }
}
