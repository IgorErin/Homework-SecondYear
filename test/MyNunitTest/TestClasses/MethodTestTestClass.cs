namespace MyNunitTest.TestClasses;

using MyNunit.Exceptions;

/// <summary>
/// Class for test use only.
/// </summary>
public class MethodTestTestClass
{
    /// <summary>
    /// Test that should pass.
    /// </summary>
    [MyNunit.Attributes.Test]
    public void SomeTestShouldPass()
    {
        Task.Delay(100).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Ignored test method.
    /// </summary>
    [MyNunit.Attributes.Test(Ignore = "never mind")]
    public void IgnoreTest()
    {
        throw new FailException();
    }

    /// <summary>
    /// Test that should fail.
    /// </summary>
    [MyNunit.Attributes.Test]
    public void TestShouldFail()
    {
        throw new FailException();
    }

    /// <summary>
    /// Test that should passed with expected exception.
    /// </summary>
    [MyNunit.Attributes.Test(Expected = typeof(FailException))]
    public void ExpectedExceptionTest()
    {
        throw new FailException();
    }
}