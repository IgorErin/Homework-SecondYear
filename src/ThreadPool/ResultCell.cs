using Optional;

namespace ThreadPool;

public class ResultCell<TResult>
{
    private readonly Func<TResult> _func;

    private Option<TResult> _result = Option.None<TResult>();
    private Option<Exception> _exception = Option.None<Exception>();
    
    private readonly ActionState _actionState;

    private volatile ResultCellStatus _cellStatus;
    
    public TResult Result
    {
        get
        {
            _actionState.GetResultBlocking();
            
        }
    }

    public ResultCell(Func<TResult> func, ActionState actionState)
    {
        _func = func;
        _actionState = actionState;

        _cellStatus = ResultCellStatus.NotComputedYet;
    }

    public bool TryComputeResultInCurrentThread()
    {
        if (_actionState.TryStartAction())
        {
            ComputeResultAndSetStatus();
            
            _actionState.ReleaseResultBlocking();

            return true;
        }

        return false;
    }

    private void ComputeResultAndSetStatus()
    {
        try
        {
            _result = _func.Invoke().Some();
            _cellStatus = ResultCellStatus.SuccessfullyComputed;
        }
        catch (Exception e)
        {
            _exception = e.Some();
            _cellStatus = ResultCellStatus.ComputedWithException;
        }
    }

    private enum ResultCellStatus
    {
        NotComputedYet,
        SuccessfullyComputed,
        ComputedWithException,
    }
}
