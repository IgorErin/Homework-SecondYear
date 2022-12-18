namespace MyNunitTest;

using System.Reflection;
using MyNunit;
using MyNunit.Tests.MethodTest;
using TestClasses;

/// <summary>
/// <see cref="MyNunit"/> tests.
/// </summary>
public class MyNunitTest
{
    /// <summary>
    /// <see cref="MethodTest.Status"/> property test.
    /// </summary>
    /// <param name="methodName">Test method name.</param>
    /// <param name="status">Expected result status.</param>
    [Test]
    [TestCase("IgnoreTest", MethodTestStatus.IgnoredWithMessage)]
    [TestCase("ExpectedExceptionTest", MethodTestStatus.ReceivedExpectedException)]
    [TestCase("TestShouldFail", MethodTestStatus.ReceivedUnexpectedException)]
    [TestCase("SomeTestShouldPass", MethodTestStatus.Passed)]
    public void ExpectedTest(string methodName, MethodTestStatus status)
    {
        var typeInstance = Activator.CreateInstance(typeof(MethodTestTestClass));
        var methodInfo = typeInstance?.GetType().GetMethod(methodName) ?? throw new NullReferenceException("method null");

        var methodTest = new MethodTest(typeInstance, Array.Empty<MethodInfo>(), methodInfo, Array.Empty<MethodInfo>());
        methodTest.Run();

        Assert.That(methodTest.Status, Is.EqualTo(status));
    }
}
