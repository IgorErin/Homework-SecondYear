using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace Lazy.Utils;

public static class EnumerableExtensions
{
    public static int DuplicatesGroupCount<T>(this IEnumerable<T> groups)
        => groups.GroupBy(x => x).Count();

    public static bool HaveNullItem<T>(this IEnumerable<T> elements) where T : class?
        => elements.Any(x => x is null);
}