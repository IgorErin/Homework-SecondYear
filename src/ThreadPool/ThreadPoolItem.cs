using System.Collections.Concurrent;

namespace ThreadPool;

/// <summary>
/// Class representing a thread in a <see cref="MyThreadPool"/>.
/// </summary>
internal class ThreadPoolItem
{
    private readonly Thread _thread;

    private readonly BlockingCollection<Action> _queue;

    private readonly CancellationToken _token;

    private readonly CountdownEvent _countdownEvent;

    /// <summary>
    /// Class constructor.
    /// </summary>
    /// <param name="queue">
    /// Queue from which the thread will retrieve and execute actions, see <see cref="BlockingCollection{T}"/>
    /// </param>
    /// <param name="countdown">
    /// Synchronization primitive for concurrent completion of threads, see <see cref="CountdownEvent"/>
    /// </param>
    /// <param name="token">Token indicating completion, see <see cref="CancellationToken"/></param>
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
                if (_token.IsCancellationRequested)
                {
                    break;
                }

                var action = _queue.Take(_token);

                action();
            }
        }
        catch (OperationCanceledException _)
        {
            // In case of throwing an exit exception.
        }
        finally
        {
            _countdownEvent.Signal();
        }
    }
}
