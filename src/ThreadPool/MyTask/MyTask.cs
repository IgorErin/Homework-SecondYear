namespace ThreadPool.MyTask;

public class MyTask<TResult> : IMyTask<TResult>
{
    private readonly ResultCell<TResult> _resultCell;
    private readonly ThreadPool _threadPull;
    
    private readonly object _locker = new object();

    public bool IsCompleted
        => _resultCell.IsCompleted();

    public TResult Result
        => _resultCell.GetResult();

    public MyTask(ResultCell<TResult> resultCell, ThreadPool threadPool)
    {
        _resultCell = resultCell;
        _threadPull = threadPool;
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
        => _threadPull.Submit(() => continuation.Invoke(this.Result)); //TODO()
}
