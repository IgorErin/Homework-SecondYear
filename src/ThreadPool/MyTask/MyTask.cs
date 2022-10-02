using System.Collections.Concurrent;

namespace ThreadPool.MyTask;

public class MyTask<TResult> : IMyTask<TResult>
{
    private readonly ResultCell<TResult> _resultCell;
    private readonly MyThreadPool _myThreadPull;

    private readonly BlockingCollection<Action> _continueWith;
    private readonly ActionState _actionState;

    private readonly object _locker;

    public bool IsCompleted
    {
        get => false;
    }

    public TResult Result
        => default(TResult);

    public MyTask(ResultCell<TResult> resultCell, ActionState actionState, MyThreadPool threadPool)
    {
        _resultCell = resultCell;
        _actionState = actionState; 

        _continueWith = new BlockingCollection<Action>();
        _locker = new object();
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
    {
        if (!IsCompleted)
        {
            
            _continueWith.Add();
        }
    }

    private TResult GetResultWithBlock()
    {
        
    }
    
}
