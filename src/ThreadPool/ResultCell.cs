using Optional;

namespace ThreadPool;

public enum CellStatus // internal ?
{
    ResultNotComputed,
    ResultSuccessfullyComputed,
    ComputedWithException,
}

public class ResultCell<TResult>
{
    private readonly Func<TResult> _func;
    
    private Option<TResult> _optionResult = Option.None<TResult>();
    private Option<Exception> _optionException = Option.None<Exception>();

    private volatile bool _funcIsComputed = false;

    private volatile CellStatus _cellStatus = CellStatus.ResultNotComputed;

    public bool IsComputed
    {
        get => _funcIsComputed;
    }

    public TResult Result
    {
        get => _optionResult.ValueOr(() => throw new Exception()); //TODO()
    }

    public Exception Exception
    {
        get => _optionException.ValueOr(() => throw new Exception()); //TODO()
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
}
