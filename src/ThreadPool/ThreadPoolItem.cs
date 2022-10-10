using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ThreadPool;

/// <summary>
/// Class representing a thread in a <see cref="MyThreadPool"/>.
/// </summary>
internal class ThreadPoolItem
{
    private readonly Thread _thread;

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
        
        _thread = new Thread(() => ThreadWork());
        _thread.Start();
    }

    /// <summary>
    /// Block current thread until <see cref="ThreadPoolItem"/> not completed.
    /// </summary>
    public void Join()
    {
        _thread.Join();
    }

    private void ThreadWork()
    {
        foreach (var action in _queue)
        {
            action();
        }
        
        _countdownEvent.Signal(); // is it enough for correct blocking in ShutDown?
    }
}
