namespace ThreadPool.MyTask;

public class MyTask<TResult> : IMyTask<TResult>
{
    private readonly ResultCell<TResult> _resultCell;
    private readonly MyThreadPool _myThreadPull;
    
    private readonly object _locker = new object();

    public bool IsCompleted
        => _resultCell.IsCompleted();

    public TResult Result
        => _resultCell.GetResult();

    public MyTask(ResultCell<TResult> resultCell, MyThreadPool myThreadPool)
    {
        _resultCell = resultCell;
        _myThreadPull = myThreadPool;
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
        => _myThreadPull.Submit(() => continuation.Invoke(this.Result)); //TODO()
}
