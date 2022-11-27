namespace MyNunit.Attributes;

/// <summary>
/// Before test attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeAttribute : Attribute
{
}