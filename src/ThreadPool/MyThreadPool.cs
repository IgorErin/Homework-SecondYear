using System.Collections.Concurrent;
using ThreadPool.MyTask;

namespace ThreadPool;

public class MyThreadPool : IDisposable
{
    private readonly int _threadCount;
    private readonly ThreadPoolItem[] _threads;

    private readonly BlockingCollection<Action> _queue;

    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly CountdownEvent _countdownEvent;

    private bool _disposed;

    private readonly object _locker = new ();

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
    
    public MyTask<T> Submit<T>(Func<T> func)
    {
        var resultCell = new ResultCell<T>(func);
        
        var newTask = new MyTask<T>(this, resultCell); 

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
        
        _queue.Add(newAction);

        return newTask; 
    }

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

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _queue.Dispose();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        lock (_locker)
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
