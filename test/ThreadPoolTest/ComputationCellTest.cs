using System;
using System.Threading;
using ThreadPool.Common;
using ThreadPool.Exceptions;

namespace ThreadPool;

/// <summary>
/// <see cref="ComputationCell{TResult}"/> nunit test class.
/// </summary>
public class ComputationCellTest
{
    private ComputationCell<int> _computationResulCell = new (() => 2 * 2);
    private ComputationCell<int> _computationCellWithExceptionResult = new (() => throw new TestException());

    private const int FirstResult = 4;

    [SetUp]
    public void SetUp()
    {
        _computationResulCell = new ComputationCell<int>(() => FirstResult);
        _computationCellWithExceptionResult = new ComputationCell<int>(() => throw new TestException());
    }

    
    /// <summary>
    /// <see cref="ComputationCell{TResult}.Result"/> test.
    /// </summary>
    [Test]
    public void GetResultTest()
    {
        _computationResulCell.Compute();
            
        var result = _computationResulCell.Result;
        
        Assert.That(result, Is.EqualTo(FirstResult));
    }
    
    /// <summary>
    /// <see cref="ComputationCell{TResult}.Result"/> test without first calling
    /// <see cref="ComputationCell{TResult}.Compute()"/>.
    /// </summary>
    [Test]
    public void GetResultWithoutComputeMethod()
    {
        try
        {
            var result = _computationResulCell.Result;
            
            Assert.Fail();
        }
        catch (Exception e)
        {
            Assert.That(e, Is.InstanceOf(typeof(ComputationCellException)));
        }
    }

    /// <summary>
    /// <see cref="ComputationCell{TResult}.Compute()"/> in another thread test.
    /// </summary>
    [Test]
    public void ComputationInAnotherThreadTest()
    {
        var localComputationCell = new ComputationCell<int>(() => FirstResult);
        var thread = new Thread(() =>
            {
                lock (localComputationCell)
                {
                    if (!localComputationCell.IsComputed)
                    {
                        localComputationCell.Compute();
                    }
                }
            });
        
        thread.Start();
        thread.Join();

        var result = localComputationCell.IsComputed;
        
        Assert.That(result, Is.EqualTo(true));
    }

    /// <summary>
    /// Number of calling <see cref="ComputationCell{TResult}.Compute()"/> in another threads test.
    /// </summary>
    [Test]
    public void NumberOfComputationInAnotherThreadsTest()
    {
        var processorCount = Environment.ProcessorCount;
        var threads = new Thread[processorCount];

        const int iterationNumber = 100;

        for (var i = 0; i < iterationNumber; i++)
        {
            var testObject = new object();
            var newComputationCell = new ComputationCell<object>(() => testObject);

            for (var j = 0; j < processorCount; j++)
            {
                threads[j] = new Thread(() =>
                {
                    newComputationCell.Compute();
                });
            }

            threads.StartAndJoinAllThreads();

            var result = newComputationCell.Result;
            
            Assert.That(result, Is.EqualTo(testObject));
        }
    }

    /// <summary>
    /// <see cref="ComputationCell{TResult}.Result()"/> throw computed exception test.
    /// </summary>
    [Test]
    public void ExceptionInComputationCellTest()
    {
        try
        {
            _computationCellWithExceptionResult.Compute();
            
            var result = _computationCellWithExceptionResult.Result;
            
            Assert.Fail();
        }
        catch (Exception e)
        {
            Assert.That(e, Is.InstanceOf(typeof(TestException)));
        }
    }
    
    /// <summary>
    /// The number of exceptions thrown by the <see cref="ComputationCell{TResult}.Result()"/> in other threads.
    /// </summary>
    /// <exception cref="TestException"></exception>
    [Test]
    public void NumberOfExceptionsInAnotherThreads()
    {
        var processorCount = Environment.ProcessorCount;
        var threads = new Thread[processorCount];

        const int iterationNumber = 100;

        for (var i = 0; i < iterationNumber; i++)
        {
            var newComputationCell = new ComputationCell<int>(() => throw new TestException());

            for (var j = 0; j < processorCount; j++)
            {
                threads[j] = new Thread(() =>
                {
                    newComputationCell.Compute();
                });
            }
            
            threads.StartAndJoinAllThreads();

            try
            {
                var result = newComputationCell.Result;
            }
            catch (Exception e)
            {
                Assert.That(e, Is.InstanceOf(typeof(TestException)));
            }
        }
    }
}
