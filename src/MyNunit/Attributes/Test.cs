namespace MyNunit.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class Test : Attribute
{

    public string Ignore
    {
        get;
        set;
    }

    public Type Expected
    {
        get;
        set;
    }
}
