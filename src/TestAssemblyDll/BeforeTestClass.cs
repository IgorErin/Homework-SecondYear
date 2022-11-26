namespace TestAssemblyDll;

using MyNunit.Attributes;

public class BeforeTestClass
{
    public bool isBeforeRun;

    [Before]
    public void BeforeTestMethod()
    {
        this.isBeforeRun = true;
    }

    [Test]
    public void SomeTestMethod()
    {
        throw new Exception();
    }
}