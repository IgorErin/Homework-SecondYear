namespace MyNunitTest.TestClasses;

using MyNunit.Attributes;
using MyNunit.Exceptions;

public class AfterTestClass
{
    public static bool isRun;

    [After]
    public void AfterTestMethod()
    {
        isRun = true;
    }

    [Test(Ignore = "before test attribute test")]
    public void SomeTest()
    {
        throw new FailException();
    }
}