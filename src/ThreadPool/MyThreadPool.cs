namespace ThreadPool;

using System.Collections.Concurrent;
using Optional;
using Exceptions;
using MyTask;

/// <summary>
/// Thread pool that implement <see cref="IDisposable"/>.
/// </summary>
public sealed class MyThreadPool : IDisposable
{
    private readonly BlockingCollection<Action> queue;

    private readonly CountdownEvent threadsCompletedEvent;

    private readonly object locker = new ();

    private volatile bool isDisposed;
    private volatile bool isShutDown;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="threadCount">
    /// Number strictly greater than zero - number of threads processing tasks.
    /// </param>
    public MyThreadPool(int threadCount)
    {
        if (threadCount <= 0)
        {
            throw new MyThreadPoolException(
                $"the number of threads must be greater than 0, but was = {threadCount}");
        }

        this.queue = new BlockingCollection<Action>();

        this.threadsCompletedEvent = new CountdownEvent(threadCount);

        for (var i = 0; i < threadCount; i++)
        {
            var _ = new ThreadPoolItem(this.queue, this.threadsCompletedEvent);
        }
    }

    /// <summary>
    /// A method that allows you to add a task for execution in the form of <see cref="Func{TResult}"/>.
    /// </summary>
    /// <param name="func">Function whose value will be computed as one.</param>
    /// <typeparam name="TResult">Function result type.</typeparam>
    /// <returns>Abstraction over the task accepted for execution, see <see cref="IMyTask{TResult}"/>.</returns>
    public MyTask<TResult> Submit<TResult>(Func<TResult> func)
    {
        var resultTask = Option.None<MyTask<TResult>>();

        try
        {
            Monitor.Enter(this.locker);

            if (!this.isShutDown)
            {
                var (newTask, newCell) = MyTaskFactory.CreateNewTaskAndCell(func, this);

                resultTask = newTask.Some<MyTask<TResult>>();

                this.queue.Add(() => newCell.Compute());
            }
        }
        catch (Exception e)
        {
            throw new MyThreadPoolException($"submit error:{e.Message}", e);
        }
        finally
        {
            if (Monitor.IsEntered(this.locker))
            {
                Monitor.Exit(this.locker);
            }
        }

        return resultTask.ValueOr(() => throw new MyThreadPoolException("submit error, task not added)"));
    }

    /// <summary>
    /// The blocking thread method in which it will be called,
    /// stopping the work of threads,
    /// all tasks accepted for execution are calculated.
    /// </summary>
    public void ShutDown()
    {
        lock (this.locker)
        {
            if (!this.isShutDown)
            {
                return;
            }

            this.queue.CompleteAdding();

            this.threadsCompletedEvent.Wait();

            this.isShutDown = true;
        }
    }

    /// <summary>
    /// Method to release managed resources.
    /// </summary>
    public void Dispose()
    {
        if (!this.isShutDown)
        {
            this.ShutDown();
        }

        lock (this.locker)
        {
            if (!this.isDisposed)
            {
                this.queue.Dispose();
            }

            this.isDisposed = true;
        }
    }

    /// <summary>
    /// A method that allows you to put the calculation of the <see cref="ComputationCell{TResult}"/>
    /// on the <see cref="MyThreadPool"/> without blocking.
    ///
    /// </summary>
    /// <param name="cell"><see cref="ComputationCell{TResult}"/> for calculations.</param>
    /// <typeparam name="T">Type of ComputationCell.</typeparam>
    public void EnqueueCell<T>(ComputationCell<T> cell)
    {
        try
        {
            this.queue.Add(() => cell.Compute());
        }
        catch (ObjectDisposedException e)
        {
            throw new MyThreadPoolException("the shutdown is called, the task cannot be completed", e);
        }
        catch (InvalidOperationException e)
        {
            throw new MyThreadPoolException("the shutdown is called, the task cannot be completed", e);
        }
    }
}
