using System.Diagnostics;
using System.Reflection;
//TODO()

namespace MyNunit.Method;

public class MethodTest
{
    private readonly Stopwatch stopwatch;
    private readonly MethodInfo methodInfo;

    private readonly AdditionalMethods beforeMethods;
    private readonly AdditionalMethods afterMethods;

    public MethodTest(Stopwatch stopwatch, MethodInfo methodInfo, AdditionalMethods before, AdditionalMethods after)
    {
        this.stopwatch = stopwatch;
        this.methodInfo = methodInfo;

        this.beforeMethods = before;
        this.afterMethods = after;

        this.RunTest();
    }

    private void RunTest()
    {
        //TODO() run tests.
    }
}