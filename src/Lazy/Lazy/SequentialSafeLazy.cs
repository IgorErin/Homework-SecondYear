using Lazy.LazyExceptions;

namespace Lazy.Lazy;

public class SequentialSafeLazy<T> : Lazy<T>
{
    private readonly Func<T> _func;
    
    private Func<T> _computedValue;
    private ComputationStatus _computeStatus;
    private Exception _computedException;

    public SequentialSafeLazy(Func<T> func)
    {
        _func = func;
        
        _computeStatus = ComputationStatus.NotComputedYet;
        _computedValue = () => throw new NotComputedValueLazyException("expression not computed yet");
        _computedException = new NotCachedExceptionLazyException();
    }

    public override T Get()
        => _computeStatus switch
        {
            ComputationStatus.NotComputedYet => ExecuteFirstComputationAndGetResult(),
            ComputationStatus.SuccessComputed => GetComputedValue(),
            ComputationStatus.ComputedWithException => ThrowComputedException(),
            _ => throw new NotImplementedException()
        };

    private T ExecuteFirstComputationAndGetResult()
    {
        ComputeFuncAndSetValueAndStatus();

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
