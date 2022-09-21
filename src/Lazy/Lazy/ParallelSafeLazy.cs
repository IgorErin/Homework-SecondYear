using Lazy.LazyExceptions;

namespace Lazy.Lazy;

public class ParallelSafeLazy<T> : Lazy<T>
{
    private readonly Func<T> _func;
    
    private volatile Func<T> _computedValue;
    private volatile ComputationStatus _computeStatus;
    private volatile Exception _computedException;

    private readonly object _locker;
    
    public ParallelSafeLazy(Func<T> func)
    {
        _func = func;
        
        _computeStatus = ComputationStatus.NotComputedYet;
        _computedValue = () => throw new NotComputedValueLazyException("expression not computed yet");
        _computedException = new NotCachedExceptionLazyException();

        _locker = new object();
    }

    public override T Get()
        => _computeStatus switch
        {
            ComputationStatus.NotComputedYet => ExecuteFirsComputationLockedAndGetResult(),
            ComputationStatus.SuccessComputed => GetComputedValue(),
            ComputationStatus.ComputedWithException => ThrowComputedException(),
            _ => throw new NotImplementedException()
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
