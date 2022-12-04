namespace ThreadPool;

using Extensions;
using Common;

/// <summary>
/// <see cref="ComputationCell{TResult}"/> nunit test class.
/// </summary>
public class ComputationCellTest
{
    private const int FirstResult = 4;

    private ComputationCell<int> computationResulCell = new (() => 2 * 2);
    private ComputationCell<int> computationCellWithExceptionResult = new (() => throw new TestException());

    /// <summary>
    /// Set up method.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.computationResulCell = new ComputationCell<int>(() => FirstResult);
        this.computationCellWithExceptionResult = new ComputationCell<int>(() => throw new TestException());
    }

    /// <summary>
    /// <see cref="ComputationCell{TResult}.Result"/> test.
    /// </summary>
    [Test]
    public void GetResultTest()
    {
        this.computationResulCell.Compute();

        var result = this.computationResulCell.Result;

        Assert.That(result, Is.EqualTo(FirstResult));
    }

    /// <summary>
    /// <see cref="ComputationCell{TResult}.Compute()"/> in another thread test.
    /// </summary>
    [Test]
    public void ComputationInAnotherThreadTest()
    {
        var localComputationCell = new ComputationCell<int>(() => 1);
        var thread = new Thread(() =>
            {
                localComputationCell.Compute();
            });

        thread.Start();
        thread.Join();

        var result = localComputationCell.IsComputed;

        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Number of calling <see cref="ComputationCell{TResult}.Result"/> in another threads test.
    /// </summary>
    [Test]
    public void NumberOfComputationInAnotherThreadsTest()
    {
        const int iterationNumber = 100;

        var threads = new Thread[iterationNumber];

        for (var i = 0; i < iterationNumber; i++)
        {
            var newComputationCell = new ComputationCell<object>(() => new object());
            var results = new object[iterationNumber];

            for (var j = 0; j < iterationNumber; j++)
            {
                var localIndex = j;

                threads[j] = new Thread(() =>
                {
                    results[localIndex] = newComputationCell.Result;
                });
            }

            threads.StartAndJoinAllThreads();

            Assert.That(results.IsAllTheSameAndNotNull, Is.True);
        }
    }

    /// <summary>
    /// <see cref="ComputationCell{TResult}.Result()"/> throw computed exception test.
    /// </summary>
    [Test]
    public void ExceptionInComputationCellTest()
    {
        this.computationCellWithExceptionResult.Compute();

        Assert.Throws<TestException>(() =>
        {
            this.computationCellWithExceptionResult.Result.Ignore();
        });
    }

    /// <summary>
    /// The number of exceptions thrown by the <see cref="ComputationCell{TResult}.Result()"/> in other threads.
    /// </summary>
    [Test]
    public void NumberOfExceptionsInAnotherThreads()
    {
        const int iterationNumber = 100;

        var threads = new Thread[iterationNumber];

        for (var i = 0; i < iterationNumber; i++)
        {
            var newComputationCell = new ComputationCell<int>(() => throw new TestException());

            var exceptions = new Exception[iterationNumber];

            for (var j = 0; j < iterationNumber; j++)
            {
                var localIndex = j;

                threads[j] = new Thread(() =>
                {
                    try
                    {
                        newComputationCell.Result.Ignore();
                    }
                    catch (TestException currentException)
                    {
                        exceptions[localIndex] = currentException;
                    }
                });
            }

            threads.StartAndJoinAllThreads();

            Assert.That(exceptions.IsAllTheSameAndNotNull(), Is.True);
        }
    }
}
