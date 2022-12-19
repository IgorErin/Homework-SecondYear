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
    /// <typeparam name="T">Type of ignored value.</typeparam>
    public static void Ignore<T>(this T someObject)
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
            lazy.Value.Ignore();
        }
        catch
        {
        }
    }
}
