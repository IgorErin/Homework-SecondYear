namespace ThreadPool.MyTask;

public class MyTask<TResult> : IMyTask<TResult>
{
    private readonly ResultCell<TResult> _resultCell;
    private readonly MyThreadPool _threadPool;

    public bool IsCompleted
    {
        get => _resultCell.IsComputed;
    }

    public TResult Result
    {
        get
        {
            if (!_resultCell.IsComputed)
            {
                ComputeResultInCurrentThread();
            }

            return GetResult();
        }
    }

    public MyTask(MyThreadPool threadPool, ResultCell<TResult> resultCell)
    {
        _resultCell = resultCell;
        _threadPool = threadPool;
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
    {
        Func<TNewResult> newFunc;
        
        if (_resultCell.IsComputed)
        {
            newFunc = () => continuation.Invoke(GetResult());
            
            return _threadPool.Submit(newFunc);
        }

        newFunc = () =>
        {
            var result = Result;

            return continuation.Invoke(result);
        };

        return _threadPool.Submit(newFunc);
    }

    private TResult GetResult()
        => _resultCell.Status switch
        {
            ResultCell<TResult>.CellStatus.ResultSuccessfullyComputed => _resultCell.Result,
            ResultCell<TResult>.CellStatus.ComputedWithException => throw GetResultException(),
            _ => throw new Exception() //TODO()
        };

    private AggregateException GetResultException()
    {
        return new AggregateException(_resultCell.Exception);
    }

    private void ComputeResultInCurrentThread()
    {
        lock (_resultCell.Locker)
        {
            if (!_resultCell.IsComputed)
            {
                _resultCell.Compute();
            }
        }
    }
}
