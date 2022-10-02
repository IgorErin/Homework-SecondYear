using System.Collections.Concurrent;

namespace ThreadPool;

internal class ThreadPoolItem
{
    private readonly Thread _thread;
    private volatile ThreadState _threadState;

    private readonly BlockingCollection<Action> _queue;

    private readonly CancellationToken _token;

    public ThreadPoolItem(BlockingCollection<Action> queue, CancellationToken token)
    {
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

                action();
            }
        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine("Cancellation exception are thrown");
        }
    }
}