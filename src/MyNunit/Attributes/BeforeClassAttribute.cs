namespace MyNunit.Attributes;

/// <summary>
/// Before class attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeClassAttribute : Attribute
{
}