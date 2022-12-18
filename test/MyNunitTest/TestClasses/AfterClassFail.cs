namespace MyNunitTest.TestClasses;

using Common;
using MyNunit.Attributes;

/// <summary>
/// <see cref="AfterClassAttribute"/> class for tests.
/// </summary>
public class AfterClassFail
{
    /// <summary>
    /// Method for tests.
    /// </summary>
    [AfterClass]
    public static void AfterFailMethod()
    {
        throw new TestException();
    }
}