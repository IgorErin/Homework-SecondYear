namespace ThreadPool.MyTask;

public interface IMyTask<out TResult>
{

    public bool IsCompleted
    {
        get;
    }

    public TResult Result
    {
        get;
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation);
}