namespace MyNunitTest.TestClasses;

using MyNunit.Exceptions;

public class ExpectedTestClass
{
    [MyNunit.Attributes.Test(Expected = typeof(FailException))]
    public void ExpectedExceptionTest()
    {
        throw new FailException();
    }
}