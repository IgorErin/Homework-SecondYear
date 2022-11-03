namespace LazyTest;

using System;
using Lazy.Lazy;
using Utils;
using NUnit.Framework;

/// <summary>
/// Nunit test class with test methods.
/// </summary>
public class UnsafeLazyTest
{
    private const int ResultCount = 10;

    /// <summary>
    /// Test method checking that the value is evaluated once and always the same.
    /// </summary>
    [Test]
    public void MultipleLazyGetResultsAreEqualsTest()
    {
        var resultArray = new object[ResultCount];

        var seqLazy = new UnsafeLazy<object>(() => new object());

        for (var i = 0; i < ResultCount; i++)
        {
            resultArray[i] = seqLazy.Get();
        }

        var groupsCount = resultArray.DuplicatesGroupCount();

        Assert.AreEqual(1, groupsCount);
    }

    /// <summary>
    /// Method checking for throwing an exception: the calculation is unique and the exception is always the same.
    /// </summary>
    /// <exception cref="Exception">An exception is thrown and immediately caught to check for identity.</exception>
    [Test]
    public void LazyComputationExceptionsAreEqualsTest()
    {
        var exceptions = new Exception[ResultCount];

        var parLazy = new SafeLazy<Exception>(() => throw new Exception());

        for (var i = 0; i < ResultCount; i++)
        {
            try
            {
                parLazy.Get();
            }
            catch (Exception exception)
            {
                exceptions[i] = exception;
            }
        }

        var groupsCount = exceptions.DuplicatesGroupCount();

        Assert.AreEqual(1, groupsCount);
    }

    /// <summary>
    /// Test method for checking if an exception is thrown in lazy computation.
    /// </summary>
    /// <exception cref="Exception">Checked exception.</exception>
    [Test]
    public void LazyExceptionAreThrown()
    {
        var parLazy = new SafeLazy<Exception>(() => throw new Exception());

        var exceptionAreThrown = false;

        for (var i = 0; i < ResultCount; i++)
        {
            try
            {
                parLazy.Get();
            }
            catch (Exception _)
            {
                exceptionAreThrown = true;
            }
        }

        Assert.True(exceptionAreThrown);
    }

    /// <summary>
    /// Test method for checking the identity of lazy evaluation and regular evaluation.
    /// </summary>
    [Test]
    public void LazyAndSimpleLoopSumResultAreEqualsTest()
    {
        var func = () =>
        {
            var sum = 0;
            const int sumCount = 10;

            for (var i = 0; i < sumCount; i++)
            {
                sum += i;
            }

            return sum;
        };

        var simpleResult = func.Invoke();

        var lazy = new UnsafeLazy<int>(func);
        var lazyResult = lazy.Get();

        Assert.AreEqual(simpleResult, lazyResult);
    }
}
