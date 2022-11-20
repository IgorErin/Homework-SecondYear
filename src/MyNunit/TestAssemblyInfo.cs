namespace MyNunit;

public class TestAssemblyInfo
{
    private readonly long time;
    private readonly List<TestClassInfo> tests;

    public long Time => this.time;

    public List<TestClassInfo> Tests => tests;

    public TestAssemblyInfo(long time, List<TestClassInfo> tests)
    {
        this.time = time;
        this.tests = tests;
    }
}
