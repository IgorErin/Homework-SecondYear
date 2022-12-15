namespace ThreadPool;

/// <summary>
/// Class containing definitions of common types.
/// </summary>
public static class Type
{
    private static readonly Unit unit = new ();

    /// <summary>
    /// Gets <see cref="Type.UnitType"/>.
    /// </summary>
    public static Unit UnitType => unit;

    /// <summary>
    /// Type of alias for <see cref="Void"/>.
    /// </summary>
    public class Unit
    {
    }
}