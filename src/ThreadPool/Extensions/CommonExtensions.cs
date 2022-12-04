namespace ThreadPool.Extensions;

/// <summary>
/// Class contains common extension.
/// </summary>
public static class CommonExtensions
{
    /// <summary>
    /// Ignore method.
    /// </summary>
    /// <param name="someObject">Some object to ignore.</param>
    public static void Ignore(this object someObject)
    {
        var _ = someObject;
    }
}
