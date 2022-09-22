using System.Threading;

namespace Lazy.Utils;

public static class ThreadArrayExtensions
{
    public static void StartAll(this Thread[] threads)
    {
        foreach (var thread in threads)
        {
            thread.Start();
        }
    }

    public static void JoinAll(this Thread[] threads)
    {
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}