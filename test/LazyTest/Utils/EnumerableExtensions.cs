namespace LazyTest.Utils;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// <see cref="Enumerable"/> extensions class.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Duplicates group count.
    /// </summary>
    /// <param name="groups">Collection to count groups.</param>
    /// <typeparam name="T">Type of collection elements.</typeparam>
    /// <returns>Group count.</returns>
    public static int DuplicatesGroupCount<T>(this IEnumerable<T> groups)
        => groups.GroupBy(x => x).Count();

    /// <summary>
    /// Function - predicate.
    /// </summary>
    /// <param name="elements">Collection to search for zero.</param>
    /// <typeparam name="T">Type of collection elements.</typeparam>
    /// <returns>True if collection contain null, otherwise - false.</returns>
    public static bool HaveNullItem<T>(this IEnumerable<T> elements)
        where T : class?
        => elements.Any(x => x is null);
}
