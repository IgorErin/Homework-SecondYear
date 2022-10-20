using System.Collections.Concurrent;

namespace ThreadPool;

/// <summary>
/// Class representing a thread in a <see cref="MyThreadPool"/>.
/// </summary>
internal class ThreadPoolItem
{
    private readonly BlockingCollection<Action> _queue;

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
    public ThreadPoolItem(BlockingCollection<Action> queue, CountdownEvent countdown)
    {
        _queue = queue;
        
        _countdownEvent = countdown;
        
        new Thread(() => ThreadWork()).Start();
    }

    private void ThreadWork()
    {
        foreach (var action in _queue.GetConsumingEnumerable())
        {
            action();
        }

        _countdownEvent.Signal(); // is it enough for correct blocking in ShutDown?
    }
}
