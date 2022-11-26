namespace MyNunitTest.TestClasses;

using MyNunit.Exceptions;

public class IgnoreTestClass
{
    [MyNunit.Attributes.Test(Ignore = "never mind")]
    public void IgnoreTest()
    {
        throw new FailExceptions();
    }
}
