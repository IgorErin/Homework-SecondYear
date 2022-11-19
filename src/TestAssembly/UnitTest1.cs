namespace TestAssembly;

[Obsolete]
public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ShouldPass()
    {
        Assert.Pass();
    }

    [Test]
    public void TestShouldFail()
    {
        Assert.That(1, Is.EqualTo(-1));
    }
}