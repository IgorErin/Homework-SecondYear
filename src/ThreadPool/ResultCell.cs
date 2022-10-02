using Optional;
using ThreadPool.Exceptions;

namespace ThreadPool.MyTask;

public class ResultCell<TResult>
{
    private readonly Func<TResult> _func;
    
    private Option<TResult> _optionResult = Option.None<TResult>();
    private Option<Exception> _optionException = Option.None<Exception>();

    private volatile bool _funcIsComputed;

    private volatile CellStatus _cellStatus = CellStatus.ResultNotComputed;
    
    public bool IsComputed
    {
        get => _funcIsComputed;
    }

    public TResult Result
    {
        get 
            => _optionResult.ValueOr(
                () => throw new ResultCellException("result value not init")
                );
    }

    public Exception Exception
    {
        get => _optionException.ValueOr(
                () => throw new ResultCellException("result exception not init")
                );
    }

    public CellStatus Status
    {
        get => _cellStatus;
    }

    public ResultCell(Func<TResult> func)
    {
        _func = func;
    }

    public void Compute()
    {
        try
        {
            _optionResult = _func.Invoke().Some();
            _cellStatus = CellStatus.ResultSuccessfullyComputed;
        }
        catch (Exception e)
        {
            _optionException = e.Some();
            _cellStatus = CellStatus.ComputedWithException;
        }
        finally
        {
            _funcIsComputed = true;
        }
    }
    
    public enum CellStatus // internal ?
    {
        ResultNotComputed,
        ResultSuccessfullyComputed,
        ComputedWithException,
    }
}
