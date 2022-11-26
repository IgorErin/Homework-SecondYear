namespace MyNunit;

using System.Reflection;
using System.Text;

public class TestAssemblyInfo
{
    private readonly long time;
    private readonly List<TestClassInfo> testsClasses;
    private readonly Assembly assembly;

    public long Time => this.time;

    public List<TestClassInfo> TestsClasses => testsClasses;

    public TestAssemblyInfo(long time, List<TestClassInfo> testsClasses, Assembly assembly)
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
            stringBuilder.Append(testClass.ToString());
        }

        return stringBuilder.ToString();
    }
}
