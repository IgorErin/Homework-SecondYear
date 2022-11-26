namespace MyNunitTest;

using MyNunit;
using MyNunit.TestsInfo;
using MyNunit.Attributes;
using TestClasses;

/// <summary>
/// <see cref="MyNunit"/> tests.
/// </summary>
public class MyNunitTest
{
    /// <summary>
    /// <see cref="BeforeClassAttribute"/> test.
    /// </summary>
    [NUnit.Framework.Test]
    public void BeforeClassTest()
    {
        var _ = MyNunit.RunTypeTests(typeof(BeforeClassTest));

        Assert.That(TestClasses.BeforeClassTest.isRun, Is.True);
    }

    /// <summary>
    /// <see cref="BeforeAttribute"/> test.
    /// </summary>
    [NUnit.Framework.Test]
    public void BeforeMethodTest()
    {
        var _ = MyNunit.RunTypeTests(typeof(BeforeTestClass));

        Assert.That(BeforeTestClass.isRun, Is.True);
    }

    /// <summary>
    /// <see cref="AfterAttribute"/> test.
    /// </summary>
    [NUnit.Framework.Test]
    public void AfterMethodTest()
    {
        var _ = MyNunit.RunTypeTests(typeof(AfterTestClass));

        Assert.That(AfterTestClass.isRun, Is.True);
    }

    /// <summary>
    /// <see cref="AfterClassAttribute"/> test.
    /// </summary>
    [NUnit.Framework.Test]
    public void AfterClassTest()
    {
        var _ = MyNunit.RunTypeTests(typeof(AfterClassTest));

        Assert.That(TestClasses.AfterClassTest.isRun, Is.True);
    }

    /// <summary>
    /// <see cref="IgnoreAttribute"/> test.
    /// </summary>
    [NUnit.Framework.Test]
    public void IgnoreTest()
    {
        var testInfo = MyNunit.RunTypeTests(typeof(IgnoreTestClass));

        Assert.That(testInfo.Tests[0].Result.Item3, Is.EqualTo(TestStatus.Ignored));
    }

    /// <summary>
    /// <see cref="MyNunit"/> Expected property test.
    /// </summary>
    [NUnit.Framework.Test]
    public void ExpectedTest()
    {
        var testInfo = MyNunit.RunTypeTests(typeof(ExpectedTestClass));

        Assert.That(testInfo.Tests[0].Result.Item3, Is.EqualTo(TestStatus.Passed));
    }

    /// <summary>
    /// <see cref="MyNunit"/> failed test.
    /// </summary>
    [NUnit.Framework.Test]
    public void FailedTest()
    {
        var testInfo = MyNunit.RunTypeTests(typeof(FailTestClass));

        Assert.That(testInfo.Tests[0].Result.Item3, Is.EqualTo(TestStatus.Failed));
    }

    /// <summary>
    /// <see cref="MyNunit"/> passed test.
    /// </summary>
    [NUnit.Framework.Test]
    public void PassTest()
    {
        var testInfo = MyNunit.RunTypeTests(typeof(PassTestClass));

        Assert.That(testInfo.Tests[0].Result.Item3, Is.EqualTo(TestStatus.Passed));
    }
}
