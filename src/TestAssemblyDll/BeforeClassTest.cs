namespace TestAssemblyDll;

using MyNunit.Attributes;
using MyNunit.Exceptions;

public class BeforeClassTest
{
    public bool isBeforeTestRun;

    [BeforeClass]
    public void BeforeTestMethod()
    {
        this.isBeforeTestRun = true;
    }

    [Test]
    public void TestShouldFail()
    {
        throw new FailExceptions();
    }
}