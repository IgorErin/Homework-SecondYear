namespace MyNunitTest;

using MyNunit.Attributes;
using MyNunit.Tests.TypeTest;
using TestClasses;

/// <summary>
/// <see cref="TypeTest"/> test class.
/// </summary>
public class TypeTestTest
{
    /// <summary>
    /// <see cref="BeforeClassAttribute"/> test.
    /// </summary>
    [NUnit.Framework.Test]
    public void BeforeClassTest()
    {
        new TypeTest(typeof(BeforeClassTest)).Run();

        Assert.That(TestClasses.BeforeClassTest.IsRun, Is.True);
    }

    /// <summary>
    /// <see cref="BeforeAttribute"/> test.
    /// </summary>
    [NUnit.Framework.Test]
    public void BeforeMethodTest()
    {
        new TypeTest(typeof(BeforeTest)).Run();

        Assert.That(BeforeTest.IsRun, Is.True);
    }

    /// <summary>
    /// <see cref="AfterAttribute"/> test.
    /// </summary>
    [NUnit.Framework.Test]
    public void AfterMethodTest()
    {
        new TypeTest(typeof(AfterTest)).Run();

        Assert.That(AfterTest.IsRun, Is.True);
    }

    /// <summary>
    /// <see cref="AfterClassAttribute"/> test.
    /// </summary>
    [NUnit.Framework.Test]
    public void AfterClassTest()
    {
        new TypeTest(typeof(AfterClassTest)).Run();

        Assert.That(TestClasses.AfterClassTest.IsRun, Is.True);
    }

    /// <summary>
    /// <see cref="TypeTest.Status"/> test.
    /// </summary>
    /// <param name="type">Type to test.</param>
    /// <param name="status">Expected result test status.</param>
    [Test]
    [TestCase(typeof(BeforeClassFail), TypeTestStatus.BeforeFailed)]
    [TestCase(typeof(AfterClassFail), TypeTestStatus.AfterFailed)]
    public void StatusPropertyTest(Type type, TypeTestStatus status)
    {
        var test = new TypeTest(type);

        test.Run();

        Assert.That(test.Status, Is.EqualTo(status));
    }
}