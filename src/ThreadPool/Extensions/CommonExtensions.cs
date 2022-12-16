namespace ThreadPool.Extensions;

/// <summary>
/// Class contains common extension.
/// </summary>
public static class CommonExtensions
{
    /// <summary>
    /// Ignore method.
    /// </summary>
    /// <remarks>
    /// Value types will be boxed.
    /// </remarks>
    /// <param name="someObject">Some object to ignore.</param>
    public static void Ignore(this object? someObject)
    {
        var _ = someObject;
    }

    /// <summary>
    /// Compute lazy.
    /// </summary>
    /// <param name="lazy">Lazy to compute.</param>
    /// <typeparam name="T">Lazy type.</typeparam>
    public static void Compute<T>(this Lazy<T> lazy)
    {
        try
        {
            lazy.Value.Ignore(); // boxing...
        }
        catch
        {
        }
    }
}
