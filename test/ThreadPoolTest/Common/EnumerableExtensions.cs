namespace ThreadPool.Common;

/// <summary>
/// <see cref="Enumerable"/> extension class.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Method that returns the number of duplicate groups.
    /// </summary>
    /// <param name="groups"><see cref="IEnumerable{T}"/> collection.</param>
    /// <typeparam name="T">Type of collection items.</typeparam>
    /// <returns>Group of duplications count.</returns>
    public static int DuplicatesGroupCount<T>(this IEnumerable<T> groups)
        => groups.GroupBy(x => x).Count();

    /// <summary>
    /// Method that checks for the presence of null in the collection.
    /// </summary>
    /// <param name="elements"><see cref="IEnumerable{T}"/> collection.</param>
    /// <typeparam name="T">Type of collection items.</typeparam>
    /// <returns>True if there is null in the collection, otherwise false.</returns>
    public static bool HaveNullItem<T>(this IEnumerable<T> elements)
        where T : class?
        => elements.Any(x => x is null);

    /// <summary>
    /// A method that checks that all the elements in the collection are the same and that they are not nulls.
    /// </summary>
    /// <param name="elements"><see cref="IEnumerable{T}"/> collection.</param>
    /// <typeparam name="T">Type of collection items.</typeparam>
    /// <returns>True if all elements are the same and not nulls, otherwise false.</returns>
    public static bool IsAllTheSameAndNotNull<T>(this IEnumerable<T> elements)
        where T : class?
        => !elements.HaveNullItem() && elements.DuplicatesGroupCount() == 1;
}
