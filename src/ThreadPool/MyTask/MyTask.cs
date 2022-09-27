using Optional;

namespace ThreadPool.MyTask;

public class MyTask<T> : IMyTask<T>
{
    private readonly Func<T> _func;
    
    private Option<T> _optionResult;
    private Option<AggregateException> _resultException;

    private volatile TaskStatus _taskStatus;

    private readonly object _locker;

    public bool IsCompleted
        => _taskStatus switch
        {
            TaskStatus.ValueComputed => true,
            TaskStatus.ComputedException => true,

            _ => false
        };
    
    public T Result
        => _taskStatus switch
        {
            TaskStatus.ValueNotComputedYet => ComputeSafetySetStatusAndGetResult(),
            TaskStatus.ValueComputed => GetComputedValue(),
            TaskStatus.ComputedException => throw GetComputedException(),
            
            _ => throw new Exception() //TODO()
        };

    public MyTask(Func<T> func)
    {
        _func = func;
        
        _optionResult = Option.None<T>();
        _resultException = Option.None<AggregateException>();
        
        _taskStatus = TaskStatus.ValueNotComputedYet;

        _locker = new object();
    }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<T, TNewResult> continuation)
        => new MyTask<TNewResult>(() => continuation.Invoke(Result));

    private T ComputeSafetySetStatusAndGetResult()
    {
        lock (_locker)
        {
            if (_taskStatus == TaskStatus.ValueNotComputedYet)
            {
                ComputeAndSetStatus();
            }
            else
            {
                // comment needed;
            }
        }

        return Result;
    }

    private Exception GetComputedException()
        => _resultException.Match(
            some: x => throw x,
            none: () => new Exception()//TODO()
        ); 

    private T GetComputedValue()
        => _optionResult.ValueOr(() => throw new Exception()); //TODO()

    private void ComputeAndSetStatus()
    {
        try
        {
            var result = _func.Invoke();
            _optionResult = Option.Some(result);

            _taskStatus = TaskStatus.ValueComputed;
        }
        catch (Exception funcException)
        {
            var aggregateException = new AggregateException(funcException);
            _resultException = Option.Some(aggregateException);

            _taskStatus = TaskStatus.ComputedException;
        }
    }

    private enum TaskStatus
    {
        ValueNotComputedYet,
        ValueComputed,
        ComputedException
    }
}
