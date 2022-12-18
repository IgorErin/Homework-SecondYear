namespace MyNunitTest.TestClasses;

using Common;
using MyNunit.Attributes;

/// <summary>
/// <see cref="BeforeClassAttribute"/> test class.
/// </summary>
public class BeforeClassFail
{
    /// <summary>
    /// <see cref="BeforeClassAttribute"/> test method.
    /// </summary>
    [BeforeClass]
    public static void BeforeFailMethod()
    {
        throw new TestException();
    }
}