namespace MyNunit;

public interface ITestPrinter
{
    public void PrintAssemblyTest();

    public void PrintTypeTest();

    public void PrintMethodTest();
}