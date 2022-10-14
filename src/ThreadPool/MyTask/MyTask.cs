using System.Collections.Concurrent;
using ThreadPool.Exceptions;

namespace ThreadPool.MyTask;

/// <summary>
/// Class that implement <see cref="IMyTask{TResult}"/>, <inheritdoc cref="IMyTask{TResult}"/>
/// </summary>
/// <typeparam name="TResult">Result type.</typeparam>
public class MyTask<TResult> : IMyTask<TResult>
{
    private readonly ComputationCell<TResult> _computationCell;
    private readonly MyThreadPool _threadPool;

    private readonly BlockingCollection<Action> _actions; 

    public bool IsCompleted
    {
        get => _computationCell.IsComputed;
    }
    
    /// <summary>
    /// A property that returns the result of the evaluation or,
    /// if it has not yet been evaluated,
    /// blocks the calling thread until it is evaluated and returns the value.
    /// </summary>
    /// <exception cref="MyTaskException">Will be thrown on runtime error.</exception>
    /// <exception cref="AggregateException">
    /// Will be thrown at an exception in the computation, will contain it as inner exception.
    /// </exception>
    public TResult Result
    {
        get
        {
            try
            {
                return GetResultFromComputationCell();
            }
            catch (ComputationCellException e)
            {
                throw new MyTaskException("computation error: \n", e);
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
    public MyTask(MyThreadPool threadPool, ComputationCell<TResult> computationCell, BlockingCollection<Action> actions)
    {
        _computationCell = computationCell;
        _threadPool = threadPool;

        _actions = actions;
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
            var result = GetResultFromComputationCell();

            return continuation.Invoke(result);
        };

        if (IsCompleted || _actions.IsAddingCompleted)
        {
            return _threadPool.Submit(newFunc);
        }

        try
        {
            Monitor.Enter(_actions);

            if (IsCompleted || _actions.IsAddingCompleted)
            {
                return _threadPool.Submit(newFunc);
            }

            var (newTask, newCell) = MyTaskFactory.CreateNewTaskAndCell(newFunc, _threadPool);

            _actions.Add(() => _threadPool.Enqueue(newCell));

            return newTask;
        }
        catch (Exception e)
        {
            throw new MyTaskException("Continue add error: ", e);
        }
        finally
        {
            if (Monitor.IsEntered(_actions))
            {
                Monitor.Exit(_actions);
            }
        }
        
    }

    private TResult GetResultFromComputationCell()
    {
        if (!_computationCell.IsComputed)
        {
            _computationCell.Compute();
        }
        
        return _computationCell.Result;
    }
}
