namespace Queue;

public class BlockingQueue<TValue>
{
    private readonly PriorityQueue<TValue, int> queue = new (new CustomComparer());

    private readonly object locker = new ();

    public void Enqueue(TValue value, int priority)
    {
        lock (this.locker)
        {
            this.queue.Enqueue(value, priority);
            
            Monitor.PulseAll(this.locker);
        }
    }

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

    public int Size() => this.queue.Count;

    private class CustomComparer : IComparer<int>
    {
        public int Compare(int left, int right) => - left.CompareTo(right);
    }
}