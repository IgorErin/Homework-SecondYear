using ThreadPool.Exceptions;

namespace ThreadPool.MyTask;

public class MyTask<TResult> : IMyTask<TResult>
{
    private readonly ResultCell<TResult> _resultCell;
    private readonly MyThreadPool _threadPool;

    private readonly List<Exception> _exceptions = new ();

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
            try
            {
                var result = Result;

                return continuation.Invoke(result);
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
            }

            throw new AggregateException(_exceptions); //TODO()
        };

        return _threadPool.Submit(newFunc);
    }

    private TResult GetResult()
        => _resultCell.Status switch
        {
            ResultCell<TResult>.CellStatus.ResultSuccessfullyComputed => _resultCell.Result,
            ResultCell<TResult>.CellStatus.ComputedWithException => throw GetResultException(),
            
            _ => throw new MyTaskException($"status not match, status = {_resultCell.Status}")
        };

    private AggregateException GetResultException()
    {
        return new AggregateException(_resultCell.Exception);
    }

    private void ComputeResultInCurrentThread()
    {
        lock (_resultCell)
        {
            if (!_resultCell.IsComputed)
            {
                _resultCell.Compute();
            }
        }
    }
}
