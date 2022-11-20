namespace MyNunit.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : Attribute
{
    public string? Ignore
    {
        get;
        set;
    }

    public Type? Expected
    {
        get;
        set;
    }
}
