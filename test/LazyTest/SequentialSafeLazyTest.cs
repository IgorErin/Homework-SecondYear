using System;
using Lazy.Lazy;
using Lazy.Utils;
using NUnit.Framework;

namespace Lazy;

/// <summary>
/// Nunit test class with test methods.
/// </summary>
public class Tests
{
    private const int ResultCount = 10;
    
    /// <summary>
    /// Test method checking that the value is evaluated once and always the same
    /// </summary>
    [Test]
    public void MultipleLazyGetResultAreEqualsTest()
    {
        var resultArray = new object[ResultCount];
        
        var seqLazy = new SequentialSafeLazy<object>(() => new object());
        
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
    public void ExceptionLazyComputationAreEqualTest()
    {
        var exceptions = new Exception[ResultCount];

        var parLazy = new ParallelSafeLazy<Exception>(() => throw new Exception());

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
    /// Method for checking the identity of lazy evaluation and regular evaluation.
    /// </summary>
    [Test]
    public void LazyAndSimpleLoopSumResultAreEqualsTest()
    {
        var func = () =>
        {
            var sum = 0;
            var sumCount = 10;

            for (var i = 0; i < sumCount; i++)
            {
                sum += i;
            }

            return sum;
        };

        var simpleResult = func.Invoke();
        
        var lazy = new SequentialSafeLazy<int>(func);
        var lazyResult = lazy.Get();
        
        Assert.AreEqual(simpleResult, lazyResult);
    }
}