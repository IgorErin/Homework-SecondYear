namespace ThreadPool;

/// <summary>
/// Class containing definitions of common types.
/// </summary>
public static class Type
{
    private static readonly UnitType UnitInstance = new ();

    /// <summary>
    /// Gets <see cref="Unit"/>.
    /// </summary>
    public static UnitType Unit => UnitInstance;

    /// <summary>
    /// Type of alias for <see cref="Void"/>.
    /// </summary>
    public class UnitType
    {
    }
}