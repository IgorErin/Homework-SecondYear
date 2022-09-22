using System;
using System.Threading;
using Lazy.Lazy;
using Lazy.Utils;
using NUnit.Framework;

namespace Lazy;

/// <summary>
/// Nunit test class with test methods.
/// </summary>
public class ParTests
{
    private const int ThreadCount = 100;
    
    /// <summary>
    /// Test method checking that the value is evaluated once and always the same
    /// </summary>
    [Test]
    public void MultipleLazyGetResultAreEqualsTest()
    {
        var threadArray = new Thread[ThreadCount];
        var resultArray = new object[ThreadCount];
        
        var parLazy = new ParallelSafeLazy<object>(() => new object());
        
        for (var i = 0; i < ThreadCount; i++)
        {
            var localIndex = i;
            
            threadArray[i] = new Thread(() =>
            {
                var result = parLazy.Get();

                resultArray[localIndex] = result;
            });
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
        var threadArray = new Thread[ThreadCount];
        var exceptions = new Exception[ThreadCount];

        var parLazy = new ParallelSafeLazy<object>(() => throw new Exception());
        
        for (var i = 0; i < ThreadCount; i++)
        {
            var localIndex = i;
            
            threadArray[i] = new Thread(() =>
            {
                try
                {
                    var result = parLazy.Get();
                }
                catch (Exception currentException)
                {
                    exceptions[localIndex] = currentException;
                }
            });
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
        
        var lazy = new ParallelSafeLazy<int>(func);
        var lazyResult = lazy.Get();
        
        Assert.AreEqual(simpleResult, lazyResult);
    }
}