namespace MyNunitTest;

using MyNunit;
using TestAssemblyDll;

public class MyNunitTest
{
    [NUnit.Framework.Test]
    public void BeforeClassTest()
    {
        var _ = MyNunit.RunTypeTests(typeof(BeforeClassTest));

        Assert.That(TestAssemblyDll.BeforeClassTest.isRun, Is.True);
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

        Assert.That(TestAssemblyDll.AfterClassTest.isRun, Is.True);
    }
}