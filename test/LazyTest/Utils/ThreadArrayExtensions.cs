namespace LazyTest.Utils;

using System.Threading;

/// <summary>
/// Thread array extensions class.
/// </summary>
public static class ThreadArrayExtensions
{
    /// <summary>
    /// Method that <see cref="Thread.Start()"/> all the thread in array.
    /// </summary>
    /// <param name="threads">Array of thread to be started.</param>
    public static void StartAll(this Thread[] threads)
    {
        foreach (var thread in threads)
        {
            thread.Start();
        }
    }

    /// <summary>
    /// Method that <see cref="Thread.Join()"/> all threads in array.
    /// </summary>
    /// <param name="threads">Array of threads to be joined.</param>
    public static void JoinAll(this Thread[] threads)
    {
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}
