using System.Collections.Generic;
using System.Linq;

namespace Lazy.Utils;

public static class EnumerableExtensions
{
    public static int DuplicatesGroupCount<T>(this IEnumerable<T> groups)
        => groups.GroupBy(x => x).Sum(x => x.Count());
}