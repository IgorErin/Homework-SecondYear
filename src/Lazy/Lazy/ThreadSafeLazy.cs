using Lazy.LazyExceptions;
using Optional;

namespace Lazy.Lazy;

/// <summary>
/// lazy evaluation class for multi thread execution
/// <inheritdoc cref="Lazy{T}"/>
/// </summary>
/// <typeparam name="T">Result type of lazy computed expression, see <see cref="Lazy{T}"/></typeparam>
public class ThreadSafeLazy<T> : Lazy<T>
{
    private readonly Func<T> _func;
    
    private Option<T> _computedValue;
    private volatile Exception _computedException;
    
    private volatile ComputationStatus _computeStatus;

    private readonly object _locker;
    
    /// <summary>
    /// Constructor for init lazy computation class, see <see cref="ThreadSafeLazy{T}"/>.
    /// </summary>
    /// <param name="func">A function that will be lazily evaluated</param>
    public ThreadSafeLazy(Func<T> func)
    {
        _func = func;
        
        _computeStatus = ComputationStatus.NotComputedYet;
        _computedValue = Option.None<T>();
        _computedException = new NotCachedExceptionLazyException();

        _locker = new object();
    }
    
    /// <summary>
    /// A method that returns a lazily evaluated value or throws an exception.
    /// Returning the same object each time in the case of a reference type.
    /// Or thrown the same exception each time.
    /// </summary>
    /// <returns>Expression result</returns>
    /// <exception cref="ComputedStatusNotMatchLazyException">Throws an exception on status mismatch</exception>
    public override T Get()
        => _computeStatus switch
        {
            ComputationStatus.NotComputedYet => ExecuteFirsComputationLockedAndGetResult(),
            ComputationStatus.SuccessComputed => GetComputedValue(),
            ComputationStatus.ComputedWithException => ThrowComputedException(),
            _ => throw new ComputedStatusNotMatchLazyException()
        };

    private T ExecuteFirsComputationLockedAndGetResult()
    {
        lock (_locker)
        {
            if (_computeStatus == ComputationStatus.NotComputedYet)
            {
                ComputeFuncAndSetValueAndStatus();
            }
            else
            {
                // if the value is calculated in another thread,
                // then exit the lock without calculating it and return the result
            }
        }

        return Get();
    }

    private void ComputeFuncAndSetValueAndStatus()
    {
        try
        {
            var resultValue = _func.Invoke();

            _computedValue = resultValue.Some<T>();
            _computeStatus = ComputationStatus.SuccessComputed;
        }
        catch (Exception computedException)
        {
            _computedException = computedException;
            _computeStatus = ComputationStatus.ComputedWithException;
        }
    }

    private T GetComputedValue()
        => _computedValue.Match(
            some: result => result,
            none: () => throw new NotComputedResultLazyException()
            );

    private T ThrowComputedException()
        => throw _computedException;
}
