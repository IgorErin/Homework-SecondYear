using System;
using System.Collections.Concurrent;
using System.Threading;
using Optional;
using ThreadPool.Exceptions;
using ThreadPool.MyTask;
using ThreadPool;

namespace ThreadPool;

/// <summary>
/// Thread pool that implement <see cref="IDisposable"/>.
/// </summary>
public sealed class MyThreadPool : IDisposable
{
    private readonly ThreadPoolItem[] _threads;

    private readonly BlockingCollection<Action> _queue;

    private readonly CountdownEvent _countdownEvent;

    private volatile bool _disposed;
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
        
        _countdownEvent = new CountdownEvent(threadCount);

        for (var i = 0; i < threadCount; i++)
        {
            _threads[i] = new ThreadPoolItem(_queue, _countdownEvent);
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
        var newTask = Option.None<MyTask<TResult>>();
        
        var submitOptionException = Option.None<Exception>();

        lock (_locker)
        {
            if (!_isShutDown && !_disposed)
            {
                try
                {
                    var newComputationCell = new ComputationCell<TResult>(func);
                    newTask = new MyTask<TResult>(this, newComputationCell).Some<MyTask<TResult>>();
                    var newAction = () => newComputationCell.Compute();
                    
                    _queue.Add(newAction);
                }
                catch (Exception exception)
                {
                    submitOptionException = exception.Some<>();
                }
            }
        }

        submitOptionException.MatchSome(exception => 
            throw new MyThreadPoolException($"Submit error: {exception.Message}", exception)
        );

        return newTask.ValueOr(
            () => throw new MyThreadPoolException("ShutDown method was applied, adding a task is not possible")
            );
    }

    /// <summary>
    /// The blocking thread method in which it will be called,
    /// stopping the work of threads,
    /// the tasks that have begun to be calculated will be completed,
    /// other tasks will not be calculated.
    /// </summary>
    public void ShutDown()
    {
        lock (_locker)
        {
            if (!_isShutDown)
            {
                _queue.CompleteAdding();
                _countdownEvent.Wait();

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
            if (!_disposed)
            {
                _queue.Dispose();
            }

            _disposed = true;
        }
    }
}
