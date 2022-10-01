using System.Collections.Concurrent;
using ThreadPool.MyTask;

namespace ThreadPool;

internal class ThreadPoolItem
{
    private readonly Thread _thread;
    private volatile ThreadState _threadStates;

    private readonly BlockingCollection<Action> _queue;

    private readonly CancellationToken _token;

    public ThreadPoolItem(BlockingCollection<Action> queue, CancellationToken token)
    {
        _threadStates = ThreadState.Waiting;

        _queue = queue;
        
        _thread = new Thread(() => ThreadWork());
        _thread.Start();
        

        _token = token;
    }

    private void ThreadWork()
    {
        try
        {
            while (true)
            {
                Console.WriteLine($"in thread, num = {Environment.CurrentManagedThreadId}");

                if (_token.IsCancellationRequested)
                {
                    break;
                }

                var action = _queue.Take(_token);

                _threadStates = ThreadState.Work;

                action();

                _threadStates = ThreadState.Waiting;
            }
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine("Cancellation exception are thrown");
        }
    }
    
    private enum ThreadState
    {
        Waiting,
        Work
    }
}

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
        var resultCell = new ResultCell<T>();
        var newTask = new MyTask<T>(resultCell, this); //TODO

        lock (_locker) //need ?
        {
            var newAction = (() =>
            {
                var result = func();
                
                resultCell.SetResult(result);
                
            });
            
            _queue.Add(newAction);
        }
        
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

    private void ThreadWork()
    {
        //TODO()
    }
    
    /// ///////////////////////////////////////////////////////////////////
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