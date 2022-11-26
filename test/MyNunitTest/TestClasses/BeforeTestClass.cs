namespace TestAssemblyDll;

using MyNunit.Attributes;

public class BeforeTestClass
{
    public static bool isRun;

    [Before]
    public void BeforeTestMethod()
    {
        isRun = true;
    }

    [Test]
    public void SomeTestMethod()
    {
        throw new Exception();
    }
}