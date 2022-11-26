namespace TestAssemblyDll;

using MyNunit.Attributes;

public class AfterClassTest
{
    public static bool isRun;

    [AfterClass]
    public static void AfterClass()
    {
        isRun = true;
    }
}