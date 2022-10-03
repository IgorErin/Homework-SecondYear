using Optional;
using ThreadPool.Exceptions;

namespace ThreadPool.ResultCell;

/// <summary>
/// The class encapsulates the calculation, its result and state.
/// </summary>
/// <typeparam name="TResult">
/// Result type.
/// </typeparam>
public class ResultCell<TResult>
{
    private readonly Func<TResult> _func;
    
    private Option<TResult> _optionResult = Option.None<TResult>();
    private Option<Exception> _optionException = Option.None<Exception>();

    private volatile bool _funcIsComputed;

    private volatile ResultCellStatus _resultCellStatus = ResultCellStatus.ResultNotComputed;
    
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
    /// <exception cref="ResultCellException">
    /// If the result is not calculated, an exception will be thrown, see <see cref="ResultCellException"/>.
    /// </exception>
    public TResult Result
    {
        get 
            => _optionResult.ValueOr(
                () => throw new ResultCellException("result value not init")
                );
    }

    /// <summary>
    /// Property that allows you to get an exception thrown during the calculation.
    /// </summary>
    /// <exception cref="ResultCellException">
    /// An exception will be thrown if the resulting exception was not thrown, <see cref="ResultCellException"/>.
    /// </exception>
    public Exception Exception
    {
        get => _optionException.ValueOr(
                () => throw new ResultCellException("result exception not init")
                );
    }

    /// <summary>
    /// Property that allow you to get state of computation in form of <see cref="ResultCellStatus"/>
    /// </summary>
    public ResultCellStatus Status
    {
        get => _resultCellStatus;
    }

    /// <summary>
    /// Constructor of class <see cref="ResultCell{TResult}"/> 
    /// </summary>
    /// <param name="func">
    /// Function that will be computed.
    /// </param>
    public ResultCell(Func<TResult> func)
    {
        _func = func;
    }

    /// <summary>
    /// The method that calculates the result.
    /// </summary>
    public void Compute()
    {
        try
        {
            _optionResult = _func.Invoke().Some();
            _resultCellStatus = ResultCellStatus.ResultSuccessfullyComputed;
        }
        catch (Exception e)
        {
            _optionException = e.Some();
            _resultCellStatus = ResultCellStatus.ComputedWithException;
        }
        finally
        {
            _funcIsComputed = true;
        }
    }
}
