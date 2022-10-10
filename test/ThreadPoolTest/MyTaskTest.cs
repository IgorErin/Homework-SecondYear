using System;
using System.Threading;
using ThreadPool.Common;

namespace ThreadPool;

/// <summary>
/// <see cref="MyTask.MyTask{TResult}"/> Nunit test class.
/// </summary>
public class MyTaskTest
{
    private const int IterCount = 10;

    /// <summary>
    /// Correct value <see cref="MyTask.MyTask{TResult}.Result"/> result before calling
    /// <see cref="MyThreadPool.ShutDown()"/>.
    /// </summary>
    /// <param name="expectedResultValue">Expected <see cref="MyTask.MyTask{TResult}.Result"/> value.</param>
    [TestCase(-1000)]
    [TestCase(-100)]
    [TestCase(0)]
    [TestCase(1000)]
    public void CorrectValueGetResultTest(int expectedResultValue)
    {
        using var threadPool = new MyThreadPool(Environment.ProcessorCount);
        var newTask = threadPool.Submit(() => expectedResultValue);

        var result = newTask.Result;
        threadPool.ShutDown();
        
        Assert.That(result, Is.EqualTo(expectedResultValue));
    }

    /// <summary>
    /// Correct reference type <see cref="MyTask.MyTask{TResult}.Result"/> result before calling
    /// <see cref="MyThreadPool.ShutDown()"/>.
    /// </summary>
    [Test]
    public void CorrectRefTypeGetResultTest()
    {
        using var threadPool = new MyThreadPool(Environment.ProcessorCount);
        var expectedResult = new object();
        
        var newTask = threadPool.Submit(() => expectedResult);
        
        var result = newTask.Result;
        
        threadPool.ShutDown();
        
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    /// <summary>
    /// <see cref="MyTask.MyTask{TResult}.Result"/> throws test exception.
    /// </summary>
    /// <exception cref="TestException">Exception that <see cref="MyTask.MyTask{TResult}.Result"/> thrown.</exception>
    [Test]
    public void TaskExceptionResultTest()
    {
        using var threadPool = new MyThreadPool(Environment.ProcessorCount);
        var testException = new TestException();
        var newTestTask = threadPool.Submit(() =>
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
            threadPool.ShutDown();
        }
    }

    /// <summary>
    /// <see cref="MyTask.MyTask{TResult}.Result"/> gives the same object in other threads.
    /// </summary>
    [Test]
    public void ConcurrentResultsAreEqualTest()
    {
        using var threadPool = new MyThreadPool(Environment.ProcessorCount);
        var processorCount = Environment.ProcessorCount;
        var threads = new Thread[processorCount];
        
        for (var i = 0; i < IterCount; i++)
        {
            var results = new object[processorCount];
            
            var testResultObject = new object();
            var newTask = threadPool.Submit(() => testResultObject);
            
            for (var j = 0; j < processorCount; j++)
            {
                var localIndex = j;
                threads[j] = new Thread(() =>
                {
                    results[localIndex] = newTask.Result;
                });
            }
            
            threads.StartAndJoinAllThreads();
            
            Assert.True(results.IsAllEqualAndNotNull());
        }
        
        threadPool.ShutDown();
    }
    
    /// <summary>
    /// <see cref="MyTask.MyTask{TResult}.Result"/> gives the same exception in other threads.
    /// </summary>
    /// <exception cref="TestException"><see cref="MyTask.MyTask{TResult}.Result"/> exception.</exception>
    [Test]
    public void ConcurrentExceptionResultsAreEqualTest()
    {
        using var threadPool = new MyThreadPool(Environment.ProcessorCount);
        var processorCount = Environment.ProcessorCount;
        var threads = new Thread[processorCount];
        
        for (var i = 0; i < IterCount; i++)
        {
            var results = new Exception[processorCount];
            
            var testException = new TestException();
            var newTask = threadPool.Submit(() =>
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
            
            Assert.True(results.IsAllEqualAndNotNull());
        }
        
        threadPool.ShutDown();
    }

    /// <summary>
    /// The correct result of the first task in the <see cref="MyTask.MyTask{TResult}.ContinueWith{TNewResult}"/>
    /// </summary>
    /// <param name="firstTaskResultValue">Result value.</param>
    [TestCase(-1000)]
    [TestCase(-100)]
    [TestCase(0)]
    [TestCase(1000)]
    public void ContinuationValueResultTest(int firstTaskResultValue)
    {
        using var threadPool = new MyThreadPool(Environment.ProcessorCount);
        var newTestTask = threadPool.Submit(() => firstTaskResultValue);
        var testTaskContinuation = newTestTask.ContinueWith(result => result);

        var result = testTaskContinuation.Result;
        
        threadPool.ShutDown();
        
        Assert.That(result, Is.EqualTo(firstTaskResultValue));
    }
    
    /// <summary>
    /// The correct exception of the base task in the <see cref="MyTask.MyTask{TResult}.ContinueWith{TNewResult}"/>
    /// </summary>
    /// <exception cref="TestException">Base task exception.</exception>
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
        var testTaskContinuation = newTestTask.ContinueWith(result => result);

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
    
    /// <summary>
    ///  The correct exception of the continuation task in the <see cref="MyTask.MyTask{TResult}.ContinueWith{TNewResult}"/>
    /// </summary>
    /// <exception cref="TestException">Result exception.</exception>
    [Test]
    public void ExceptionInContinuationTaskTest()
    {
        using var threadPool = new MyThreadPool(Environment.ProcessorCount);
        var testException = new TestException();
        var newTestTask = threadPool.Submit(() => 0);
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
            threadPool.ShutDown();
        }
    }
}