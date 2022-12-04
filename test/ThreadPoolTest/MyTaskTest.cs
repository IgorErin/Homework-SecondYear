namespace ThreadPool;

using Extensions;
using Common;

/// <summary>
/// <see cref="MyThreadPool.MyTask{TResult}"/> Nunit test class.
/// </summary>
public class MyTaskTest
{
    private const int IterCount = 10;

    /// <summary>
    /// Correct value <see cref="MyThreadPool.MyTask{TResult}.Result"/> result before calling
    /// <see cref="MyThreadPool.ShutDown()"/>.
    /// </summary>
    /// <param name="expectedResultValue">Expected <see cref="MyThreadPool.MyTask{TResult}.Result"/> value.</param>
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
    /// Correct reference type <see cref="MyThreadPool.MyTask{TResult}.Result"/> result before calling
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
    /// <see cref="MyThreadPool.MyTask{TResult}.Result"/> gives the same object in other threads.
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

            var newTask = threadPool.Submit(() => new object());

            for (var j = 0; j < processorCount; j++)
            {
                var localIndex = j;

                threads[j] = new Thread(() =>
                {
                    results[localIndex] = newTask.Result;
                });
            }

            threads.StartAndJoinAllThreads();

            Assert.That(results.IsAllTheSameAndNotNull, Is.True);
        }
    }

    /// <summary>
    /// <see cref="MyThreadPool.MyTask{TResult}.Result"/> gives the same exception in other threads.
    /// </summary>
    /// <exception cref="TestException"><see cref="MyThreadPool.MyTask{TResult}.Result"/> exception.</exception>
    [Test]
    public void ConcurrentExceptionResultsAreEqualTest()
    {
        using var threadPool = new MyThreadPool(Environment.ProcessorCount);
        var processorCount = Environment.ProcessorCount;
        var threads = new Thread[processorCount];

        for (var i = 0; i < IterCount; i++)
        {
            var results = new Exception?[processorCount];

            var testException = new TestException();
            var newTask = threadPool.Submit<int>(() => throw testException);

            for (var j = 0; j < processorCount; j++)
            {
                var localIndex = j;

                threads[j] = new Thread(() =>
                {
                    var aggregateException = Assert.Throws<AggregateException>(() =>
                    {
                        newTask.Result.Ignore();
                    });

                    results[localIndex] = aggregateException!.InnerException;
                });
            }

            threads.StartAndJoinAllThreads();

            MyAssert.NotNullAndAllEqualsTo(results, testException);
        }
    }

    /// <summary>
    /// The correct result of the first task in the <see cref="MyThreadPool.MyTask{TResult}.ContinueWith{TNewResult}"/>.
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
        var testTaskContinuation = newTestTask.ContinueWith(result => -result);

        var result = testTaskContinuation.Result;

        threadPool.ShutDown();

        Assert.That(result, Is.EqualTo(-firstTaskResultValue));
    }

    /// <summary>
    /// The correct exception of the base task in the <see cref="MyThreadPool.MyTask{TResult}.ContinueWith{TNewResult}"/>.
    /// </summary>
    /// <exception cref="TestException">Base task exception.</exception>
    [Test]
    public void ExceptionInBaseTaskContinuationTest()
    {
        using var threadPool = new MyThreadPool(Environment.ProcessorCount);
        var testException = new TestException();

        var newTestTask = threadPool.Submit<int>(() => throw testException);
        var testTaskContinuation = newTestTask.ContinueWith(result => result);

        var aggregateException = Assert.Throws<AggregateException>(() =>
        {
            testTaskContinuation.Result.Ignore();
        });
        Assert.That(aggregateException!.InnerException, Is.EqualTo(testException));
    }

    /// <summary>
    ///  The correct exception of the continuation task in the
    /// <see cref="MyThreadPool.MyTask{TResult}.ContinueWith{TNewResult}"/>.
    /// </summary>
    /// <exception cref="TestException">Result exception.</exception>
    [Test]
    public void ExceptionInContinuationTaskTest()
    {
        using var threadPool = new MyThreadPool(Environment.ProcessorCount);
        var testException = new TestException();

        var newTestTask = threadPool.Submit(() => 0);
        var testTaskContinuation = newTestTask.ContinueWith<int>(_ => throw testException);

        var aggregateException = Assert.Throws<AggregateException>(() =>
        {
            testTaskContinuation.Result.Ignore();
        });
        Assert.That(aggregateException!.InnerException, Is.EqualTo(testException));
    }
}