using System.Collections.Concurrent;
using ThreadPool.MyTask;

namespace ThreadPool;

public class MyThreadPool : IDisposable
{
    private readonly int _threadCount;
    private readonly ThreadPoolItem[] _threads;

    private readonly BlockingCollection<Action> _queue;

    private readonly object _locker = new ();

    private readonly CancellationTokenSource _cancellationTokenSource;

    private bool _disposed;

    public MyThreadPool(int threadCount)
    {
        _queue = new BlockingCollection<Action>();
        
        _threadCount = threadCount;
        _threads = new ThreadPoolItem[threadCount];
        
        _cancellationTokenSource = new CancellationTokenSource();

        // must be after _threads init
        InitAndStartThreads();
    }
    
    public MyTask<T> Submit<T>(Func<T> func)
    {
        var taskState = new ActionState();
        var resultLazy = new Lazy<T>(func);
        
        var newTask = new MyTask<T>(resultLazy,  this); //TODO

        var newAction = (() =>
        {
            
            taskState.TaskCompleted();
        });
        
        _queue.Add(newAction);

        return newTask; //TODO()
    }

    public void ShutDown()
    {
        _cancellationTokenSource.Cancel();
    }
    
    private void InitAndStartThreads()
    {
        for (var i = 0; i < _threadCount; i++)
        {
            _threads[i] = new ThreadPoolItem(_queue, _cancellationTokenSource.Token);
        }
    }

    //////////////////////////////////////////////////////////////////////
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
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
