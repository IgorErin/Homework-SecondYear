namespace TestAssemblyDll;

using MyNunit.Attributes;

public class AfterClassTest
{
    public bool afterClassRun;

    [AfterClass]
    public void AfterClass()
    {
        this.afterClassRun = true;
    }
}