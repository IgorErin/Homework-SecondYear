using System.Collections.Concurrent;

namespace ThreadPool;

internal class ThreadPoolItem
{
    private readonly Thread _thread;

    private readonly BlockingCollection<Action> _queue;

    private readonly CancellationToken _token;

    public ThreadPoolItem(BlockingCollection<Action> queue, CancellationToken token)
    {
        _queue = queue;
        _token = token;
        
        _thread = new Thread(() => ThreadWork());
        _thread.Start();
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