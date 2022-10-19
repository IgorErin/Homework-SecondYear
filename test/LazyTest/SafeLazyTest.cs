using System;
using System.Threading;
using System.Threading.Tasks;
using Lazy.Lazy;
using Lazy.Utils;
using NUnit.Framework;

namespace Lazy;

/// <summary>
/// Nunit test class with test methods.
/// </summary>
public class SafeLazyTest
{
    private const int ThreadCount = 10;
    private Thread[] _threadArray = new Thread[ThreadCount];

    [SetUp]
    public void SetUp()
    {
        _threadArray = new Thread[ThreadCount];
    }
    
    /// <summary>
    /// Test method checking that the value is evaluated once and always the same
    /// </summary>
    [Test]
    public void MultipleLazyGetResultAreEqualsTest()
    {
        var resultArray = new object[ThreadCount];
        
        var parLazy = new SafeLazy<object>(() => new object());
        
        for (var i = 0; i < ThreadCount; i++)
        {
            var localIndex = i;
            
            _threadArray[i] = new Thread(() =>
            {
                var result = parLazy.Get();

                resultArray[localIndex] = result;
            });
        }
        
        _threadArray.StartAll();
        _threadArray.JoinAll();

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
        var exceptions = new Exception[ThreadCount];

        var parLazy = new SafeLazy<object>(() => throw new Exception());
        
        for (var i = 0; i < ThreadCount; i++)
        {
            var localIndex = i;
            
            _threadArray[i] = new Thread(() =>
            {
                try
                {
                    parLazy.Get();
                }
                catch (Exception currentException)
                {
                    exceptions[localIndex] = currentException;
                }
            });
        }
        
        _threadArray.StartAll();
        _threadArray.JoinAll();

        var groupsCount = exceptions.DuplicatesGroupCount();

        Assert.AreEqual(1, groupsCount);
    }

    /// <summary>
    /// Test method that check for throwing an exception in parallel lazy evaluation.
    /// </summary>
    /// <exception cref="Exception"></exception>
    [Test]
    public void ExceptionIsThrownInParallelTest()
    {
        var exceptions = new Exception[ThreadCount];

        var parLazy = new SafeLazy<object>(() => throw new Exception());
        
        for (var i = 0; i < ThreadCount; i++)
        {
            var localIndex = i;
            
            _threadArray[i] = new Thread(() =>
            {
                try
                {
                    for (int i = 0; i < 100000; i++)
                    {
                        Task.Delay(1000000);
                    }
                    
                    var result = parLazy.Get();
                }
                catch (Exception currentException)
                {
                    exceptions[localIndex] = currentException;
                }
            });
        }
        
        _threadArray.StartAll();
        _threadArray.JoinAll();

        Assert.False(exceptions.HaveNullItem());
    }
    
    /// <summary>
    /// Test method checking that the value is evaluated in threads.
    /// </summary>
    [Test]
    public void MultipleLazyGetResultTest()
    {
        var resultArray = new object[ThreadCount];
        
        var parLazy = new SafeLazy<object>(() => new object());
        
        for (var i = 0; i < ThreadCount; i++)
        {
            var localIndex = i;
            
            _threadArray[i] = new Thread(() =>
            {
                var result = parLazy.Get();

                resultArray[localIndex] = result;
            });
        }
        
        _threadArray.StartAll();
        _threadArray.JoinAll();

        Assert.False(resultArray.HaveNullItem());
    }
}