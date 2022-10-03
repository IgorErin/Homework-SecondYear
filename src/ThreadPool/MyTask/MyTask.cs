using ThreadPool.Exceptions;
using ThreadPool.ResultCell;

namespace ThreadPool.MyTask;

/// <summary>
/// Class that implement <see cref="IMyTask{TResult}"/>, <inheritdoc cref="IMyTask{TResult}"/>
/// </summary>
/// <typeparam name="TResult">Result type.</typeparam>
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
    
    /// <summary>
    /// Class constructor.
    /// </summary>
    /// <param name="threadPool">Thread pool that perform computations</param>
    /// <param name="resultCell">Cell that encapsulates calculation and state of result</param>
    public MyTask(MyThreadPool threadPool, ResultCell<TResult> resultCell)
    {
        _resultCell = resultCell;
        _threadPool = threadPool;
    }

    /// <summary>
    /// The method that allows you to get the continuation of the result in the form <see cref="IMyTask{TResult}"/>.
    /// </summary>
    /// <param name="continuation">
    /// The function to be curried function by the previous result.
    /// </param>
    /// <typeparam name="TNewResult">
    /// Result type of curried function.
    /// </typeparam>
    /// <returns>
    /// Abstraction over computation in form of <see cref="IMyTask{TResult}"/>
    /// </returns>
    /// <exception cref="AggregateException">
    /// An exception will be thrown when the exception is thrown in the original task
    /// or its continuation, when the <see cref="Result"/> property is applied to it.
    /// </exception>
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
            ResultCellStatus.ResultSuccessfullyComputed => _resultCell.Result,
            ResultCellStatus.ComputedWithException => throw GetResultException(),
            
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
