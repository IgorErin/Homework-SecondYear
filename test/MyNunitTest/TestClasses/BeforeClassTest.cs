namespace MyNunitTest.TestClasses;

using MyNunit.Attributes;
using MyNunit.Exceptions;

public class BeforeClassTest
{
    public static bool isRun;

    [BeforeClass]
    public static void BeforeTestMethod()
    {
        isRun = true;
    }

    [Test]
    public void TestShouldFail()
    {
        throw new FailExceptions();
    }
}