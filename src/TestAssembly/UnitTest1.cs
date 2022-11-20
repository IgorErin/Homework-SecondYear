namespace TestAssembly;

using MyNunit.Attributes;

public class Tests
{
    [Before]
    public void Setup()
    {
        Console.WriteLine("before");
    }

    [Test(Ignore = "never mind")]
    public void ShouldPass()
    {
        Console.WriteLine("ShouldPass test");
    }

    [Test]
    public void TestShouldFail()
    {
        Assert.Fail();
    }

    [After]
    public void After()
    {
        Console.WriteLine("after");
    }
}