namespace ThreadPool.MyTask;

using System.Collections.Concurrent;
using Exceptions;

/// <summary>
/// Class that implement <see cref="IMyTask{TResult}"/>.<inheritdoc cref="IMyTask{TResult}"/>
/// </summary>
/// <typeparam name="TResult">Result type.</typeparam>
public class MyTask<TResult> : IMyTask<TResult>
{
    private readonly ComputationCell<TResult> computationCell;
    private readonly MyThreadPool threadPool;

    private readonly BlockingCollection<Action> actions;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTask{TResult}"/> class.
    /// </summary>
    /// <param name="threadPool">Thread pool that perform computations.</param>
    /// <param name="computationCell">Cell that encapsulates calculation and state of result.</param>
    /// <param name="actions">Actions that will be performed immediately after the completion of the task.</param>
    public MyTask(MyThreadPool threadPool, ComputationCell<TResult> computationCell, BlockingCollection<Action> actions)
    {
        this.computationCell = computationCell;
        this.threadPool = threadPool;

        this.actions = actions;
    }

    /// <summary>
    /// Gets a value indicating whether task is completed.
    /// </summary>
    public bool IsCompleted => this.computationCell.IsComputed;

    /// <summary>
    /// Gets the result of the evaluation or, if it has not yet been evaluated,
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
                return this.GetResultFromComputationCell();
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
    /// The method that allows you to get the continuation of the result in the form <see cref="IMyTask{TResult}"/>.
    /// </summary>
    /// <param name="continuation">
    /// The function to be curried function by the previous result.
    /// </param>
    /// <typeparam name="TNewResult">
    /// Result type of curried function.
    /// </typeparam>
    /// <returns>
    /// Abstraction over computation in form of <see cref="IMyTask{TResult}"/>.
    /// </returns>
    /// <exception cref="AggregateException">
    /// An exception will be thrown when the exception is thrown in the original task
    /// or its continuation, when the <see cref="Result"/> property is applied to it.
    /// </exception>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
    {
        var newFunc = () =>
        {
            var result = this.GetResultFromComputationCell();

            return continuation.Invoke(result);
        };

        if (this.IsCompleted || this.actions.IsAddingCompleted)
        {
            return this.threadPool.Submit(newFunc);
        }

        try
        {
            Monitor.Enter(this.actions);

            if (this.IsCompleted || this.actions.IsAddingCompleted)
            {
                return this.threadPool.Submit(newFunc);
            }

            var (newTask, newCell) = MyTaskFactory.CreateContinuation(newFunc, this.threadPool);

            this.actions.Add(() => newCell.Compute());

            return newTask;
        }
        catch (Exception e)
        {
            throw new MyTaskException("Continue add error: ", e);
        }
        finally
        {
            if (Monitor.IsEntered(this.actions))
            {
                Monitor.Exit(this.actions);
            }
        }
    }

    private TResult GetResultFromComputationCell()
    {
        if (!this.computationCell.IsComputed)
        {
            this.computationCell.Compute();
        }

        return this.computationCell.Result;
    }
}
