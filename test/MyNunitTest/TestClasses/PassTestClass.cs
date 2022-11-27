namespace MyNunitTest.TestClasses;

using MyNunit.Exceptions;

public class PassTestClass
{
    [MyNunit.Attributes.Test]
    public void SomeTestShouldPass()
    {
        if (1 == 1)
        {
            throw new SuccessException();
        }
    }
}