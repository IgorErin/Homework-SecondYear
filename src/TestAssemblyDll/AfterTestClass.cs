namespace TestAssemblyDll;

using MyNunit.Attributes;
using MyNunit.Exceptions;

public class AfterTestClass
{
    public bool afterIsRun;

    [After]
    public void AfterTestMethod()
    {
        this.afterIsRun = true;
    }

    [Test(Ignore = "before test attribute test")]
    public void SomeTest()
    {
        throw new FailExceptions();
    }
}