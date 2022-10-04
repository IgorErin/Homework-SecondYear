namespace ThreadPool.Common;

public static class EnumerableExtensions
{
    public static int DuplicatesGroupCount<T>(this IEnumerable<T> groups)
        => groups.GroupBy(x => x).Count();

    public static bool HaveNullItem<T>(this IEnumerable<T> elements) where T : class?
        => elements.Any(x => x is null);

    public static bool IsAllEqualAndNotNull<T>(this IEnumerable<T> elements) where T : class?
        => !elements.HaveNullItem() && elements.DuplicatesGroupCount() == 1;
}
