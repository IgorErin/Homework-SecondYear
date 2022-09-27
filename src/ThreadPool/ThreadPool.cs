using ThreadPool.MyTask;

namespace ThreadPool;

public class ThreadPool
{
    private readonly Thread[] _threads;
    private readonly Queue<IMyTask<Object>> _funcQueue;

    public ThreadPool(int threadNum)
    {
        _threads = new Thread[threadNum];

        StartAll(_threads);
        
        _funcQueue = new Queue<IMyTask<Object>>();
    }

    private void StartAll(Thread[] threads)
    {
        foreach (var thread in threads)
        {
            thread.Start();
        }
    }
}