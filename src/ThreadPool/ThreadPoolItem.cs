using System.Collections.Concurrent;

namespace ThreadPool;

internal class ThreadPoolItem
{
    private readonly Thread _thread;

    private readonly BlockingCollection<Action> _queue;

    private readonly CancellationToken _token;

    private readonly CountdownEvent _countdownEvent;

    public ThreadPoolItem(BlockingCollection<Action> queue, CountdownEvent countdown, CancellationToken token)
    {
        _queue = queue;
        _countdownEvent = countdown;
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
        finally
        {
            _countdownEvent.Signal();
        }
    }
}