namespace MyNunit.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class Test : Attribute
{
    public Test()
    {
    }

    public Test(string ignore)
    {
    }

    public Test(Exception Expected)
    {
    }
}
