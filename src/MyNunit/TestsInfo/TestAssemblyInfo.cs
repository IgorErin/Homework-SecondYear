namespace MyNunit.TestsInfo;

using System.Reflection;
using System.Text;

public class TestAssemblyInfo
{
    private readonly long time;
    private readonly IEnumerable<TestClassInfo> testsClasses;
    private readonly Assembly assembly;

    public long Time => this.time;

    public IEnumerable<TestClassInfo> TestsClasses => testsClasses;

    public TestAssemblyInfo(long time, IEnumerable<TestClassInfo> testsClasses, Assembly assembly)
    {
        this.time = time;
        this.testsClasses = testsClasses;
        this.assembly = assembly;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"Test assembly: {this.assembly.FullName}.");
        stringBuilder.AppendLine($"Time: {this.time}");
        stringBuilder.AppendLine();

        foreach (var testClass in testsClasses)
        {
            stringBuilder.Append(testClass);
        }

        return stringBuilder.ToString();
    }
}
