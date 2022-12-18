namespace MyNunitTest.TestClasses;

using MyNunit.Attributes;

/// <summary>
/// Class for test use only.
/// </summary>
public class BeforeTest
{
    /// <summary>
    /// Gets a value indicating whether that the tests have been run.
    /// </summary>
    public static bool IsRun
    {
        get;
        private set;
    }

    /// <summary>
    /// <see cref="BeforeAttribute"/> test method.
    /// </summary>
    [Before]
    public void BeforeTestMethod()
    {
        IsRun = true;
    }

    /// <summary>
    /// <see cref="TestAttribute"/> test method.
    /// </summary>
    [Test]
    public void SomeTestMethod()
    {
        throw new Exception();
    }
}