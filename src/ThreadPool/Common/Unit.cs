namespace ThreadPool.Common;

/// <summary>
/// Class containing common types.
/// </summary>
public static class CommonTypes
{
    private static Unit unit = new ();

    /// <summary>
    /// Gets <see cref="CommonTypes.UnitType"/>.
    /// </summary>
    public static Unit UnitType => unit;

    /// <summary>
    /// Type of alias for <see cref="Void"/>.
    /// </summary>
    public class Unit
    {
    }
}