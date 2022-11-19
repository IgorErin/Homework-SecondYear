namespace MyNunit.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : Attribute
{
    public TestAttribute()
    {
    }

    public TestAttribute(string ignore)
    {
    }

    public TestAttribute(Exception Expected)
    {
    }
}
