using System.Collections.Concurrent;
using System.Data;
using Optional;
using ThreadPool.Exceptions;
using ThreadPool.MyTask;

namespace ThreadPool;

/// <summary>
/// Thread pool that implement <see cref="IDisposable"/>.
/// </summary>
public sealed class MyThreadPool : IDisposable
{
    private readonly ThreadPoolItem[] _threads;

    private readonly BlockingCollection<Action> _queue;

    private readonly CountdownEvent _threadsCompletedEvent;

    private volatile bool _isDisposed;
    private volatile bool _isShutDown;

    private readonly object _locker = new ();

    /// <summary>
    /// Class constructor.
    /// </summary>
    /// <param name="threadCount">
    /// Number strictly greater than zero - number of threads processing tasks.
    /// </param>
    public MyThreadPool(int threadCount)
    {
        if (threadCount <= 0)
        {
            throw new MyThreadPoolException(
                $"the number of threads must be greater than 0, but was = {threadCount}"
                );
        }
        
        _queue = new BlockingCollection<Action>();
        
        _threads = new ThreadPoolItem[threadCount];
        
        _threadsCompletedEvent = new CountdownEvent(threadCount);

        for (var i = 0; i < threadCount; i++)
        {
            _threads[i] = new ThreadPoolItem(_queue, _threadsCompletedEvent);
        }
    }
    
    /// <summary>
    /// A method that allows you to add a task for execution in the form of <see cref="Func{TResult}"/>
    /// </summary>
    /// <param name="func">Function whose value will be computed as one.</param>
    /// <typeparam name="TResult">Function result type.</typeparam>
    /// <returns>Abstraction over the task accepted for execution, see <see cref="IMyTask{TResult}"/></returns>
    public MyTask<TResult> Submit<TResult>(Func<TResult> func)
    {
        var resultTask = Option.None<MyTask<TResult>>();

        try
        {
            Monitor.Enter(_locker);

            if (!_isShutDown)
            {
                var (newTask, newCell) = MyTaskFactory.CreateNewTaskAndCell(func, this);

                resultTask = newTask.Some<MyTask<TResult>>();

                _queue.Add(() => newCell.Compute());
            }
        }
        catch (Exception e)
        {
            throw new MyThreadPoolException($"submit error:{e.Message}", e);
        }
        finally
        {
            if (Monitor.IsEntered(_locker))
            {
                Monitor.Exit(_locker);
            }
        }

        return resultTask.ValueOr(() => throw new MyThreadPoolException("TODO()")); // TODO()
    }

    /// <summary>
    /// The blocking thread method in which it will be called,
    /// stopping the work of threads,
    /// the tasks that have begun to be calculated will be completed,
    /// other tasks will not be calculated. //TODO()
    /// </summary>
    public void ShutDown()
    {
        lock (_locker)
        {
            if (!_isShutDown)
            {
                _queue.CompleteAdding();
                _threadsCompletedEvent.Wait();

                foreach (var threadItem in _threads) // is it necessary ?
                {
                    threadItem.Join();
                }

                _isShutDown = true;
            }
        }
    }

    /// <summary>
    /// Method to release managed resources.
    /// </summary>
    public void Dispose()
    {
        if (!_isShutDown)
        {
            ShutDown();
        }

        lock (_locker)
        {
            if (!_isDisposed)
            {
                _queue.Dispose();
            }

            _isDisposed = true;
        }
    }

    public void Enqueue<T>(ComputationCell<T> cell)
    {
        Monitor.Enter(_locker);
        
        if (!_isShutDown)
        {
            _queue.Add(() => cell.Compute()); //TODO if dispose ?
        }

        if (Monitor.IsEntered(_locker))
        {
            Monitor.Exit(_locker);
        }
    }
}
