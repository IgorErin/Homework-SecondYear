namespace MyNunitTest.TestClasses;

using MyNunit.Attributes;
using MyNunit.Exceptions;

/// <summary>
/// <see cref="AfterAttribute"/> tests class.
/// </summary>
public class AfterTest
{
    /// <summary>
    /// Gets a value indicating whether the tests have been run.
    /// </summary>
    public static bool IsRun
    {
        get;
        private set;
    }

    /// <summary>
    /// <see cref="AfterAttribute"/> test method.
    /// </summary>
    [After]
    public void AfterTestMethod()
    {
        IsRun = true;
    }

    /// <summary>
    /// <see cref="TestAttribute"/> test method.
    /// </summary>
    [Test(Ignore = "before test attribute test")]
    public void SomeTest()
    {
        throw new FailException();
    }
}