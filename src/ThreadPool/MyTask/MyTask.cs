using ThreadPool.Exceptions;
using ThreadPool.ResultCell;

namespace ThreadPool.MyTask;

/// <summary>
/// Class that implement <see cref="IMyTask{TResult}"/>, <inheritdoc cref="IMyTask{TResult}"/>
/// </summary>
/// <typeparam name="TResult">Result type.</typeparam>
public class MyTask<TResult> : IMyTask<TResult>
{
    private readonly ComputationCell<TResult> _computationCell;
    private readonly MyThreadPool _threadPool;

    public bool IsCompleted
    {
        get => _computationCell.IsComputed;
    }
    
    public TResult Result
    {
        get
        {
            if (!_computationCell.IsComputed)
            {
                ComputeResultInCurrentThread();
            }

            try
            {
                var result = _computationCell.Result;
                
                return result;
            }
            catch (ResultCellException e)
            {
                throw new MyTaskException($"computation error: {e.Message}", e);
            }
            catch (Exception e)
            {
                throw new AggregateException(e);
            }
        }
    }
    
    /// <summary>
    /// Class constructor.
    /// </summary>
    /// <param name="threadPool">Thread pool that perform computations</param>
    /// <param name="computationCell">Cell that encapsulates calculation and state of result</param>
    public MyTask(MyThreadPool threadPool, ComputationCell<TResult> computationCell)
    {
        _computationCell = computationCell;
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
        var newFunc = () =>
        {
            try
            {
                var result = Result;

                return continuation.Invoke(result);
            }
            catch (Exception e)
            {
                throw new AggregateException(e);
            }
        };
        
        return _threadPool.Submit(newFunc);
    }
    
    private void ComputeResultInCurrentThread()
    {
        lock (_computationCell)
        {
            if (!_computationCell.IsComputed)
            {
                _computationCell.Compute();
            }
        }
    }
}
