namespace MyNunit.TestsInfo;

using System.Reflection;
using System.Text;

/// <summary>
/// A class containing information about assembly tests.
/// </summary>
public class TestAssemblyInfo
{
    private readonly long time;
    private readonly IEnumerable<TestClassInfo> testsClasses;
    private readonly Assembly assembly;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAssemblyInfo"/> class.
    /// </summary>
    /// <param name="time">Test execution time.</param>
    /// <param name="testsClasses">Information about Classes tests.</param>
    /// <param name="assembly">Tested assembly.</param>
    public TestAssemblyInfo(long time, IEnumerable<TestClassInfo> testsClasses, Assembly assembly)
    {
        this.time = time;
        this.testsClasses = testsClasses;
        this.assembly = assembly;
    }

    /// <summary>
    /// Gets a test execution time for this assembly.
    /// </summary>
    public long Time => this.time;

    /// <summary>
    /// Gets type tests information in form of <see cref="TestClassInfo"/>.
    /// </summary>
    public IEnumerable<TestClassInfo> TestsClasses => this.testsClasses;

    /// <summary>
    /// <see cref="ToString"/> method.
    /// </summary>
    /// <returns>Test information in string form.</returns>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"Test assembly: {this.assembly.FullName}.");
        stringBuilder.AppendLine($"Time: {this.time}");
        stringBuilder.AppendLine();

        foreach (var testClass in this.testsClasses)
        {
            stringBuilder.Append(testClass);
        }

        return stringBuilder.ToString();
    }
}
