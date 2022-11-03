namespace LazyTest;

using System;
using System.Threading;
using System.Threading.Tasks;
using Lazy.Lazy;
using Utils;
using NUnit.Framework;

/// <summary>
/// Nunit test class with test methods.
/// </summary>
public class SafeLazyTest
{
    private const int ThreadCount = 10;
    private Thread[] threadArray = new Thread[ThreadCount];

    /// <summary>
    /// SetUp method.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.threadArray = new Thread[ThreadCount];
    }

    /// <summary>
    /// Test method checking that the value is evaluated once and always the same.
    /// </summary>
    [Test]
    public void MultipleLazyGetResultAreEqualsTest()
    {
        var resultArray = new object[ThreadCount];

        var lazy = new SafeLazy<object>(() => new object());

        for (var i = 0; i < ThreadCount; i++)
        {
            var localIndex = i;

            this.threadArray[i] = new Thread(() =>
            {
                var result = lazy.Get();

                resultArray[localIndex] = result;
            });
        }

        this.threadArray.StartAll();
        this.threadArray.JoinAll();

        Assert.IsTrue(resultArray.EveryoneIsTheSame());
    }

    /// <summary>
    /// Method checking for throwing an exception: the calculation is unique and the exception is always the same.
    /// </summary>
    /// <exception cref="Exception">An exception is thrown and immediately caught to check for identity.</exception>
    [Test]
    public void ExceptionLazyComputationAreEqualTest()
    {
        var exceptions = new Exception[ThreadCount];

        var lazy = new SafeLazy<object>(() => throw new Exception());

        for (var i = 0; i < ThreadCount; i++)
        {
            var localIndex = i;

            this.threadArray[i] = new Thread(() =>
            {
                try
                {
                    lazy.Get();
                }
                catch (Exception currentException)
                {
                    exceptions[localIndex] = currentException;
                }
            });
        }

        this.threadArray.StartAll();
        this.threadArray.JoinAll();

        Assert.IsTrue(exceptions.EveryoneIsTheSame());
    }

    /// <summary>
    /// Test method that check for throwing an exception in parallel lazy evaluation.
    /// </summary>
    /// <exception cref="Exception">Test exception.</exception>
    [Test]
    public void ExceptionIsThrownInParallelTest()
    {
        var exceptions = new Exception[ThreadCount];

        var lazy = new SafeLazy<object>(() => throw new Exception());

        for (var i = 0; i < ThreadCount; i++)
        {
            var localIndex = i;

            this.threadArray[i] = new Thread(() =>
            {
                try
                {
                    for (var j = 0; j < 100000; j++)
                    {
                        Task.Delay(1000000);
                    }

                    var _ = lazy.Get();
                }
                catch (Exception currentException)
                {
                    exceptions[localIndex] = currentException;
                }
            });
        }

        this.threadArray.StartAll();
        this.threadArray.JoinAll();

        Assert.False(exceptions.HaveNullItem());
    }

    /// <summary>
    /// Test method checking that the value is evaluated in threads.
    /// </summary>
    [Test]
    public void MultipleLazyGetResultTest()
    {
        var resultArray = new object[ThreadCount];

        var lazy = new SafeLazy<object>(() => new object());

        for (var i = 0; i < ThreadCount; i++)
        {
            var localIndex = i;

            this.threadArray[i] = new Thread(() =>
            {
                var result = lazy.Get();

                resultArray[localIndex] = result;
            });
        }

        this.threadArray.StartAll();
        this.threadArray.JoinAll();

        Assert.False(resultArray.HaveNullItem());
    }

    /// <summary>
    /// Number of increment thread test.
    /// </summary>
    [Test]
    public void NumberOfIncrementThreadTest()
    {
        const int currentThreadCount = 100;
        var countEvent = new CountdownEvent(currentThreadCount);

        var incrementValue = 0;
        var lazy = new SafeLazy<int>(() => Interlocked.Increment(ref incrementValue));

        for (var i = 0; i < currentThreadCount; i++)
        {
            new Thread(() =>
            {
                var _ = lazy.Get();

                countEvent.Signal();
            }).Start();
        }

        countEvent.Wait();

        Assert.That(1, Is.EqualTo(incrementValue));
    }
}
