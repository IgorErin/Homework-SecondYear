using System.Threading;

namespace ThreadPool.Common;

public static class ThreadExtensions
{
    public static void StartAndJoinAllThreads(this Thread[] threads)
    {
        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}
