namespace ThreadPool.Common;

/// <summary>
/// <see cref="Thread"/> and structure that contain threads extension class.
/// </summary>
public static class ThreadExtensions
{
    /// <summary>
    /// Method that calls <see cref="Thread.Start()"/> and <see cref="Thread.Join()"/> for all items.
    /// </summary>
    /// <param name="threads">Array of <see cref="Thread"/>'s.</param>
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
