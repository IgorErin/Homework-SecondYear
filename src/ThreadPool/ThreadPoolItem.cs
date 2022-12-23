namespace ThreadPool;

using System.Collections.Concurrent;

/// <summary>
/// Class representing a thread in a <see cref="MyThreadPool"/>.
/// </summary>
internal class ThreadPoolItem
{
    private readonly BlockingCollection<Action> queue;
    private readonly CountdownEvent countdownEvent;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadPoolItem"/> class.
    /// </summary>
    /// <param name="queue">
    /// Queue from which the thread will retrieve and execute actions, see <see cref="BlockingCollection{T}"/>.
    /// </param>
    /// <param name="countdown">
    /// Synchronization primitive for concurrent completion of threads, see <see cref="CountdownEvent"/>.
    /// </param>
    public ThreadPoolItem(BlockingCollection<Action> queue, CountdownEvent countdown)
    {
        this.queue = queue;
        this.countdownEvent = countdown;

        new Thread(this.ThreadWork).Start();
    }

    private void ThreadWork()
    {
        foreach (var action in this.queue.GetConsumingEnumerable())
        {
            action();
        }

        this.countdownEvent.Signal();
    }
}
