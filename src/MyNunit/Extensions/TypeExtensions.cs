namespace MyNunit.Extensions;

/// <summary>
/// <see cref="Type"/> extensions class.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// IsEqual extension method for <see cref="Type"/>.
    /// </summary>
    /// <param name="firstType">This instance of <see cref="Type"/>.</param>
    /// <param name="secondType">Another instance of <see cref="Type"/>.</param>
    /// <returns>True if types is equals, other way false.</returns>
    public static bool IsEqual(this Type firstType, Type secondType)
    {
        if (firstType.FullName != secondType.FullName)
        {
            return false;
        }

        return true;
    }
}