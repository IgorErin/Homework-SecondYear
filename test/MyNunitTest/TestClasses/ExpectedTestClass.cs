namespace MyNunitTest.TestClasses;

using MyNunit.Exceptions;

public class ExpectedTestClass
{
    [MyNunit.Attributes.Test(Expected = typeof(FailExceptions))]
    public void ExpectedExceptionTest()
    {
        throw new FailExceptions();
    }
}