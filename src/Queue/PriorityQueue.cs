namespace Queue;

/// <summary>
/// Blocking queue.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class BlockingQueue<TValue>
{
    private readonly PriorityQueue<TValue, int> queue = new (new CustomComparer());

    private readonly object locker = new ();

    /// <summary>
    /// Enqueue value with priority.
    /// </summary>
    /// <param name="value">Value to enqueue.</param>
    /// <param name="priority">Value priority.</param>
    public void Enqueue(TValue value, int priority)
    {
        lock (this.locker)
        {
            this.queue.Enqueue(value, priority);
            
            Monitor.PulseAll(this.locker);
        }
    }

    /// <summary>
    /// Dequeue value with higher priority.
    /// </summary>
    /// <remarks>
    /// If the queue is empty --- blocks the current thread until the elements appear in the queue.
    /// </remarks>
    /// <returns></returns>
    public TValue Dequeue()
    {
        try
        {
            Monitor.Enter(this.locker);

            while (this.queue.Count == 0)
            {
                Monitor.Wait(this.locker);
            }

            return this.queue.Dequeue();
        }
        finally
        {
            if (Monitor.IsEntered(this.locker))
            {
                Monitor.Exit(this.locker);
            }
        }
    }

    /// <summary>
    /// Number of items in the queue.
    /// </summary>
    /// <remarks>
    /// Not thread-safe.
    /// </remarks>
    /// <returns>
    /// Queue elements count.
    /// </returns>
    public int Size() => this.queue.Count;
    
    private class CustomComparer : IComparer<int>
    {
        /// <summary>
        /// Method that revers <see cref="int.CompareTo(int)"/>
        /// </summary>
        /// <param name="left">Left value to compare.</param>
        /// <param name="right">Right value to compare.</param>
        /// <returns>Result of comparing.</returns>
        public int Compare(int left, int right) => - left.CompareTo(right);
    }
}