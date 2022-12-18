namespace MyNunitTest.TestClasses;

using MyNunit.Attributes;
using MyNunit.Tests.TypeTest;

/// <summary>
/// <see cref="TypeTest"/>.
/// </summary>
public class AfterClassTest
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
    /// <see cref="AfterClassAttribute"/> method for tests.
    /// </summary>
    [AfterClass]
    public static void AfterClass()
    {
        IsRun = true;
    }
}