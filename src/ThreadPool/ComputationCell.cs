using Optional;
using ThreadPool.Exceptions;

namespace ThreadPool;

/// <summary>
/// The class encapsulates the calculation, its result and state.
/// </summary>
/// <typeparam name="TResult">
/// Result type.
/// </typeparam>
public class ComputationCell<TResult>
{
    private readonly Func<TResult> _func;

    private readonly Action _prevAction;

    private Option<Func<TResult>> _optionResult = Option.None<Func<TResult>>();

    private volatile bool _funcIsComputed;

    private readonly object _locker = new ();

    /// <summary>
    /// Indicate state of computation.
    /// </summary>
    public bool IsComputed
    {
        get => _funcIsComputed;
    }

    /// <summary>
    /// Property that allows you to get the result of a calculation.
    /// </summary>
    /// <exception cref="ComputationCellException">
    /// If the result is not calculated, an exception will be thrown, see <see cref="ComputationCellException"/>.
    /// </exception>
    public TResult Result
    {
        get
            => _optionResult.Match(
                some: result => result.Invoke(),
                none: () => ComputeAndGetResult()
            );
    }

    public ComputationCell(Func<TResult> func, Action prevAction)
    {
        _func = func;
        _prevAction = prevAction;
    }

    /// <summary>
    /// Constructor of class <see cref="ComputationCell{TResult}"/> 
    /// </summary>
    /// <param name="func">
    /// Function that will be computed.
    /// </param>
    public ComputationCell(Func<TResult> func) : this(func, () => { })
    {
    }

    /// <summary>
    /// The method that calculates the result.
    /// Thread safe.
    /// </summary>
    public void Compute()
    {
        lock (_locker)
        {
            if (!_funcIsComputed)
            {
                try
                {
                    var result = _func.Invoke();
                    var newResultFunc = () => result;

                    _optionResult = (newResultFunc).Some();
                }
                catch (Exception exceptionResult)
                {
                    var newExceptionFunc = new Func<TResult>(() => throw exceptionResult);

                    _optionResult = newExceptionFunc.Some();
                }
                finally
                {
                    _funcIsComputed = true;
                }
            }
        }
    }

    private TResult ComputeAndGetResult()
    {
        Compute();

        return Result;
    }
}
