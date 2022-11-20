namespace TestAssembly;

using MyNunit.Attributes;
using MyNunit.Exceptions;

public class Tests
{
    [Before]
    public void Setup()
    {
    }

    [Test(Ignore = "never mind")]
    public void ShouldPass()
    {
    }

    [Test]
    public void TestShouldFail()
    {
        Assert.Fail();
    }

    [Test(Expected = typeof(SuccessException))]
    public void SomeTest()
    {
        if (1 == 1)
        {
            throw new SuccessException();
        }
    }

    [After]
    public void After()
    {
    }
}