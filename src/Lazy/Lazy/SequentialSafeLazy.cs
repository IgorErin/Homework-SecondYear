using Lazy.LazyExceptions;
using Optional;

namespace Lazy.Lazy;

/// <summary>
/// Lazy evaluation class for single-threaded execution.
/// <inheritdoc cref="Lazy{T}"/>
/// </summary>
/// <typeparam name="T">Result type of lazy computed expression, see <see cref="Lazy{T}"/></typeparam>
public class SequentialSafeLazy<T> : Lazy<T>
{
    private readonly Func<T> _func;
    
    private Option<T> _computedResult;
    private Exception _computedException;
    private ComputationStatus _computeStatus;
    

    /// <summary>
    /// Constructor for init lazy computation class, see <see cref="ThreadSafeLazy{T}"/>.
    /// </summary>
    /// <param name="func">A function that will be lazily evaluated</param>
    public SequentialSafeLazy(Func<T> func)
    {
        _func = func;

        _computedResult = Option.None<T>();
        _computedException = new NotCachedExceptionLazyException();
        
        _computeStatus = ComputationStatus.NotComputedYet;
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
            ComputationStatus.NotComputedYet => ExecuteFirstComputationAndGetResult(),
            ComputationStatus.SuccessComputed => GetComputedValue(),
            ComputationStatus.ComputedWithException => ThrowComputedException(),
            _ => throw new ComputedStatusNotMatchLazyException()
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
            
            _computedResult = resultValue.Some<T>();
            _computeStatus = ComputationStatus.SuccessComputed;
        }
        catch (Exception computedException)
        {
            _computedException = computedException;
            _computeStatus = ComputationStatus.ComputedWithException;
        }
    }

    private T GetComputedValue()
        => _computedResult.Match(
            some: result => result,
            none: () => throw new NotComputedResultLazyException("computation error, result not computed yet")
        );

    private T ThrowComputedException()
        => throw _computedException;
}
