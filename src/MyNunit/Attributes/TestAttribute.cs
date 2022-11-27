namespace MyNunit.Attributes;

/// <summary>
/// Test attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : Attribute
{
    /// <summary>
    /// Gets or sets ignore message.
    /// </summary>
    public string? Ignore
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets and gets expected exception type.
    /// </summary>
    public Type? Expected
    {
        get;
        set;
    }
}
