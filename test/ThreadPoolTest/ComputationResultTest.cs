using ThreadPool.Exceptions;
using ThreadPool.ResultCell;

namespace ThreadPool;

public class ComputationResultTest
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

    [Test]
    public void GetResultTest()
    {
        _computationResulCell.Compute();
            
        var result = _computationResulCell.Result;
        
        Assert.That(result, Is.EqualTo(FirstResult));
    }
    
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

    [Test]
    public void NumberOfComputationInAnotherThreadsTest()
    {
        var processorCount = Environment.ProcessorCount;
        var threads = new Thread[processorCount];

        const int iterationNumber = 100;

        for (var i = 0; i < iterationNumber; i++)
        {
            var newComputationCell = new ComputationCell<int>(() => FirstResult);

            for (var j = 0; j < processorCount; j++)
            {
                threads[j] = new Thread(() =>
                {
                    newComputationCell.Compute();
                });
            }

            StartAndJoinAllThreads(threads);

            var result = newComputationCell.Result;
            
            Assert.That(result, Is.EqualTo(FirstResult));
        }
    }

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

            StartAndJoinAllThreads(threads);

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

    private void StartAndJoinAllThreads(Thread[] threads)
    {
        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    private class TestException : Exception
    {
        public TestException()
        {
        }
    }
}
