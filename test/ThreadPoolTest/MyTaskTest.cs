using ThreadPool.Common;

namespace ThreadPool;

public class MyTaskTest
{
    private const int ThreadsInThreadPoolCount = 8;
    private MyThreadPool _threadPool = new (ThreadsInThreadPoolCount);

    private const int IterCount = 10;
        [SetUp]
    public void SetUp()
    {
        _threadPool = new MyThreadPool(ThreadsInThreadPoolCount);
    }

    [TestCase(-1000)]
    [TestCase(-100)]
    [TestCase(0)]
    [TestCase(1000)]
    public void CorrectValueGetResultTest(int expectedResultValue)
    {
        var newTask = _threadPool.Submit(() => expectedResultValue);

        var result = newTask.Result;
        
        _threadPool.ShutDown();
        _threadPool.Dispose();
        
        Assert.That(result, Is.EqualTo(expectedResultValue));
    }

    [Test]
    public void CorrectRefTypeGetResultTest()
    {
        var expectedResult = new object();
        
        var newTask = _threadPool.Submit(() => expectedResult);
        
        var result = newTask.Result;
        
        _threadPool.ShutDown();
        _threadPool.Dispose();
        
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void TaskExceptionResultTest()
    {
        var testException = new TestException();
        var newTestTask = _threadPool.Submit(() =>
        {
            throw testException;
            return 0;
        });

        try
        {
            var result = newTestTask.Result;

            Assert.Fail();
        }
        catch (AggregateException e)
        {
            Assert.That(e.InnerException, Is.EqualTo(testException));
        }
        finally
        {
            _threadPool.ShutDown();
            _threadPool.Dispose();
        }
    }

    [Test]
    public void ConcurrentResultsAreEqualTest()
    {
        var processorCount = Environment.ProcessorCount;
        var threads = new Thread[processorCount];
        
        for (var i = 0; i < IterCount; i++)
        {
            var results = new object[processorCount];
            
            var testResultObject = new object();
            var newTask = _threadPool.Submit(() => testResultObject);
            
            for (var j = 0; j < processorCount; j++)
            {
                var localIndex = j;
                threads[j] = new Thread(() =>
                {
                    results[localIndex] = newTask.Result;
                });
            }
            
            threads.StartAndJoinAllThreads();
            
            Assert.False(results.HaveNullItem());
            Assert.That(results.DuplicatesGroupCount(), Is.EqualTo(1));
        }
        
        _threadPool.ShutDown();
        _threadPool.Dispose();
    }
    
    [Test]
    public void ConcurrentExceptionResultsAreEqualTest()
    {
        var processorCount = Environment.ProcessorCount;
        var threads = new Thread[processorCount];
        
        for (var i = 0; i < IterCount; i++)
        {
            var results = new Exception[processorCount];
            
            var testException = new TestException();
            var newTask = _threadPool.Submit(() =>
            {
                throw testException;
                return 0;
            });
            
            for (var j = 0; j < processorCount; j++)
            {
                var localIndex = j;
                threads[j] = new Thread(() =>
                {
                    try
                    {
                        var result = newTask.Result;
                    }
                    catch (AggregateException e)
                    {
                        results[localIndex] = e.InnerException;
                    }
                });
            }
            
            threads.StartAndJoinAllThreads();
            
            Assert.False(results.HaveNullItem());
            Assert.That(results.DuplicatesGroupCount(), Is.EqualTo(1));
        }
        
        _threadPool.ShutDown();
        _threadPool.Dispose();
    }

    [TestCase(-1000)]
    [TestCase(-100)]
    [TestCase(0)]
    [TestCase(1000)]
    public void ContinuationValueResultTest(int firstTaskResultValue)
    {
        var newTestTask = _threadPool.Submit(() => firstTaskResultValue);
        var testTaskContinuation = newTestTask.ContinueWith(result => GetContinuationIntResult(result));

        var result = testTaskContinuation.Result;
        var expectedResult = GetContinuationIntResult(firstTaskResultValue);
        
        _threadPool.ShutDown();
        _threadPool.Dispose();
        
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void ExceptionInBaseTaskContinuationTest()
    {
        using var threadPool = new MyThreadPool(Environment.ProcessorCount);
        var testException = new TestException();
        
        var newTestTask = threadPool.Submit(() =>
        {
            throw testException;
            return 0;
        });
        var testTaskContinuation = newTestTask.ContinueWith(result => GetContinuationIntResult(result));

        try
        {
            var result = testTaskContinuation.Result;

            Assert.Fail();
        }
        catch (AggregateException e)
        {
            Assert.That(e.InnerException, Is.EqualTo(testException));
        }
        finally
        {
            threadPool.ShutDown();
        }
    }
    
    [Test]
    public void ExceptionInContinuationTaskTest()
    {
        var testException = new TestException();
        var newTestTask = _threadPool.Submit(() => 0);
        var testTaskContinuation = newTestTask.ContinueWith(result =>
        {
            throw testException;
            return 0;
        });

        try
        {
            var result = testTaskContinuation.Result;

            Assert.Fail();
        }
        catch (AggregateException e)
        {
            Assert.That(e.InnerException, Is.EqualTo(testException));
        }
        finally
        {
            _threadPool.ShutDown();
            _threadPool.Dispose();
        }
    }

    private int GetContinuationIntResult(int baseTaskResult)
        => -baseTaskResult;
}