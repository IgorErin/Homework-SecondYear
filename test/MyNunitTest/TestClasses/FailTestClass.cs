namespace MyNunitTest.TestClasses;

using MyNunit.Exceptions;

public class FailTestClass
{
    [MyNunit.Attributes.Test]
    public void TestShouldFail()
    {
        throw new FailException();
    }
}