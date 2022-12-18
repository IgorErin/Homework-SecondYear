namespace MyNunitTest.TestClasses;

using MyNunit.Attributes;
using MyNunit.Exceptions;

/// <summary>
/// <see cref="BeforeClassAttribute"/> class test.
/// </summary>
public class BeforeClassTest
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
    [BeforeClass]
    public static void BeforeTestMethod()
    {
        IsRun = true;
    }

    /// <summary>
    /// <see cref="TestAttribute"/> test method.
    /// </summary>
    [Test]
    public void TestShouldFail()
    {
        throw new FailException();
    }
}