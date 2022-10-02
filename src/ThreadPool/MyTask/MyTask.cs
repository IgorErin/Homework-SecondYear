using System.Collections.Concurrent;
using Optional;

namespace ThreadPool.MyTask;

public class MyTask<TResult> : IMyTask<TResult>
{
    private readonly MyThreadPool _threadPull;
    private readonly BlockingCollection<Action> _continueWith;
    
    private volatile TaskState _taskState;
    
    private readonly Func<TResult> _funcResult;
    private Option<TResult> _result = Option.None<TResult>();
    private readonly List<Exception> _resultExceptions = new ();

    private readonly object _locker;
    
    public bool IsCompleted
    {
        get => false;
    }

    public TResult Result
    {
        get => _taskState switch
            {
                TaskState.NotComputed => ComputeWithLockSetStatusAndGetResult(),
                TaskState.SuccessfullyComputed => _result.ValueOr(() => throw new Exception()),
                TaskState.ComputedWithException => throw new AggregateException(_resultExceptions)
            };
    }

    public MyTask(MyThreadPool threadPool, Func<TResult> func)
    {
        _continueWith = new BlockingCollection<Action>();

        _threadPull = threadPool;

        _locker = new object();

        _taskState = TaskState.NotComputed;

        _funcResult = func;
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
    {
        if (_taskState == TaskState.SuccessfullyComputed)
        {
            var result = _result.ValueOr(() => throw new Exception());
            var newFunc = () =>
            {
                try
                {
                    return continuation.Invoke(result);
                }
                catch (Exception e)
                {
                    _resultExceptions.Add(e);

                    throw new AggregateException(_resultExceptions);
                }
            };
            
            return _threadPull.Submit(newFunc);
        } 
        else if (_taskState == TaskState.ComputedWithException)
        {
            throw new AggregateException(_resultExceptions);
        }

        return new MyTask<TNewResult>(null, null); //TODO()
    }
    
    private TResult ComputeWithLockSetStatusAndGetResult()
    {
        lock (_locker)
        {
            ComputeInCurrentThreadAndSetStatus();
        }

        return Result;
    }
    
    private void ComputeInCurrentThreadAndSetStatus()
    {
        if (_taskState == TaskState.NotComputed)
        {
            try
            {
                _result = _funcResult.Invoke().Some();

                _taskState = TaskState.SuccessfullyComputed;

            }
            catch (Exception e)
            {
                _resultExceptions.Add(e);

                _taskState = TaskState.ComputedWithException;
            }
        }
        else
        {
            //
        }
    }

    private enum TaskState
    {
        NotComputed,
        SuccessfullyComputed,
        ComputedWithException,
    }
}
