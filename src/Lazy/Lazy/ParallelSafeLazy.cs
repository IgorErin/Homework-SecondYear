using Lazy.LazyExceptions;

namespace Lazy.Lazy;

/// <summary>
/// lazy evaluation class for multi thread execution
/// <inheritdoc cref="Lazy{T}"/>
/// </summary>
/// <typeparam name="T">Result type of lazy computed expression, see <see cref="Lazy{T}"/></typeparam>
public class ParallelSafeLazy<T> : Lazy<T>
{
    private readonly Func<T> _func;
    
    private volatile Func<T> _computedValue;
    private volatile ComputationStatus _computeStatus;
    private volatile Exception _computedException;

    private readonly object _locker;
    
    /// <summary>
    /// Constructor for init lazy computation class, see <see cref="ParallelSafeLazy{T}"/>.
    /// </summary>
    /// <param name="func">A function that will be lazily evaluated</param>
    /// <exception cref="NotComputedValueLazyException">
    /// An exception will be thrown when trying to take a result from an uninitialized field
    /// </exception>
    public ParallelSafeLazy(Func<T> func)
    {
        _func = func;
        
        _computeStatus = ComputationStatus.NotComputedYet;
        _computedValue = () => throw new NotComputedValueLazyException("expression not computed yet");
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
                // comment needed;
            }
        }

        return Get();
    }

    private void ComputeFuncAndSetValueAndStatus()
    {
        try
        {
            var resultValue = _func.Invoke();

            _computedValue = () => resultValue;
            _computeStatus = ComputationStatus.SuccessComputed;
        }
        catch (Exception computedException)
        {
            _computedException = computedException;
            _computeStatus = ComputationStatus.ComputedWithException;
        }
    }

    private T GetComputedValue()
        => _computedValue.Invoke();

    private T ThrowComputedException()
        => throw _computedException;
}
