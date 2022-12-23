namespace ThreadPool.Common;

/// <summary>
/// Class containing methods - compositions from methods of the <see cref="Assert"/> class.
/// </summary>
public static class MyAssert
{
    /// <summary>
    /// Method that checks that all elements of the collection match the selected element
    /// and they are not nulls.
    /// </summary>
    /// <param name="collection">Collections for comparison.</param>
    /// <param name="ideal">The element to be compared with.</param>
    public static void NotNullAndAllEqualsTo(IEnumerable<object?> collection, object ideal)
    {
        foreach (var item in collection)
        {
            Assert.That(item, Is.Not.Null);
            Assert.That(item, Is.EqualTo(ideal));
        }
    }
}
