namespace MyNunitTest;

using MyNunit;
using TestClasses;

public class MyNunitTest
{
    [NUnit.Framework.Test]
    public void BeforeClassTest()
    {
        var _ = MyNunit.RunTypeTests(typeof(BeforeClassTest));

        Assert.That(TestClasses.BeforeClassTest.isRun, Is.True);
    }

    [NUnit.Framework.Test]
    public void BeforeMethodTest()
    {
        var _ = MyNunit.RunTypeTests(typeof(BeforeTestClass));

        Assert.That(BeforeTestClass.isRun, Is.True);
    }

    [NUnit.Framework.Test]
    public void AfterMethodTest()
    {
        var _ = MyNunit.RunTypeTests(typeof(AfterTestClass));

        Assert.That(AfterTestClass.isRun, Is.True);
    }

    [NUnit.Framework.Test]
    public void AfterClassTest()
    {
        var _ = MyNunit.RunTypeTests(typeof(AfterClassTest));

        Assert.That(TestClasses.AfterClassTest.isRun, Is.True);
    }

    [NUnit.Framework.Test]
    public void IgnoreTest()
    {
        var testInfo =  MyNunit.RunTypeTests(typeof(IgnoreTestClass));

        Assert.That(testInfo.Tests[0].Result.Item3, Is.EqualTo(TestStatus.Ignored));
    }

    [NUnit.Framework.Test]
    public void ExpectedTest()
    {
        var testInfo =  MyNunit.RunTypeTests(typeof(ExpectedTestClass));

        Assert.That(testInfo.Tests[0].Result.Item3, Is.EqualTo(TestStatus.Passed));
    }

    [NUnit.Framework.Test]
    public void FailedTest()
    {
        var testInfo =  MyNunit.RunTypeTests(typeof(FailTestClass));

        Assert.That(testInfo.Tests[0].Result.Item3, Is.EqualTo(TestStatus.Failed));
    }

    [NUnit.Framework.Test]
    public void PassTest()
    {
        var testInfo =  MyNunit.RunTypeTests(typeof(PassTestClass));

        Assert.That(testInfo.Tests[0].Result.Item3, Is.EqualTo(TestStatus.Passed));
    }
}