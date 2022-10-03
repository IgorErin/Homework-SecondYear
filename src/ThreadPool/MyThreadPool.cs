using System.Collections.Concurrent;
using ThreadPool.Exceptions;
using ThreadPool.MyTask;
using ThreadPool.ResultCell;

namespace ThreadPool;

/// <summary>
/// Thread pool that implement <see cref="IDisposable"/>.
/// </summary>
public sealed class MyThreadPool : IDisposable
{
    private readonly int _threadCount;
    private readonly ThreadPoolItem[] _threads;

    private readonly BlockingCollection<Action> _queue;

    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly CountdownEvent _countdownEvent;

    private volatile bool _disposed;

    private readonly object _locker = new ();

    /// <summary>
    /// Class constructor.
    /// </summary>
    /// <param name="threadCount">
    /// Number of threads processing tasks.
    /// </param>
    public MyThreadPool(int threadCount)
    {
        _queue = new BlockingCollection<Action>();
        
        _threadCount = threadCount;
        _threads = new ThreadPoolItem[threadCount];
        
        _cancellationTokenSource = new CancellationTokenSource();

        _countdownEvent = new CountdownEvent(threadCount);
        
        // must be after _threads init
        InitTreadItems();
    }
    
    /// <summary>
    /// A method that allows you to add a task for execution in the form of <see cref="Func{TResult}"/>
    /// </summary>
    /// <param name="func">Function whose value will be computed as one.</param>
    /// <typeparam name="TResult">Function result type.</typeparam>
    /// <returns>Abstraction over the task accepted for execution, see <see cref="IMyTask{TResult}"/></returns>
    public MyTask<TResult> Submit<TResult>(Func<TResult> func)
    {
        var resultCell = new ComputationCell<TResult>(func);
        
        var newTask = new MyTask<TResult>(this, resultCell); 

        var newAction = () =>
        {
            lock (resultCell)
            {
                if (!resultCell.IsComputed)
                {
                    resultCell.Compute();
                }
            }
        };

        try
        {
            _queue.Add(newAction);
        }
        catch (ObjectDisposedException e)
        {
            throw new MyThreadPoolException("The ThreadPool has been disposed. Object name: MyThreadPool.\n", e);
        }
        
        _queue.Add(newAction);

        return newTask; 
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
            _cancellationTokenSource.Cancel();

            _countdownEvent.Wait();
        }
    }
    
    private void InitTreadItems()
    {
        for (var i = 0; i < _threadCount; i++)
        {
            _threads[i] = new ThreadPoolItem(_queue, _countdownEvent, _cancellationTokenSource.Token);
        }
    }

    /// <summary>
    /// Method to release managed resources.
    /// </summary>
    public void Dispose()
    {
        ShutDown();
        
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
