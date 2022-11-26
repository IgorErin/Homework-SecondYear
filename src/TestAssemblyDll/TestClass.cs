namespace TestAssemblyDll;

using MyNunit.Attributes;
using MyNunit.Exceptions;

public class TestClass
{
    [Before]
    public void Setup()
    {

    }

    public void MethodWithoutAttribute()
    {
        throw new FailExceptions();
    }

    [Test]
    public void ShouldFail()
    {
        throw new FailExceptions();
    }

    [Test(Ignore = "never mind")]
    public void ShouldPass()
    {
        throw new FailExceptions();
    }

    [Test(Expected = typeof(NullReferenceException))]
    public void NullReferenceExpectedTest()
    {
        throw new NullReferenceException();
    }

    [After]
    public void After()
    {

    }
}