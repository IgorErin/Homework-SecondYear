namespace MyNunit;

using System.Diagnostics;
using System.Reflection;
using Attributes;

public class MyNunit
{
    private static readonly LinkedList<string> loadedPaths = new ();
    private static readonly Stopwatch stopwatch = new ();

    public MyNunit()
    {
    }

    public (IEnumerable<(long, string)>, IEnumerable<string>) RunTests(string pathToAssembly)
    {
        var assembly = Assembly.LoadFrom(pathToAssembly);
        loadedPaths.AddLast(pathToAssembly);

        foreach (var type in assembly.ExportedTypes)
        {


            foreach (var method in type.GetTypeInfo().GetMethods())
            {
                if (IsTestMethod(method))
                {

                }
            }
        }
    }

    [Test(Expected = typeof(Exception), Ignore = "some reason")]
    private bool IsTestMethod(MethodInfo methodInfo)
    {
        foreach (var att in methodInfo.CustomAttributes)
        {
            if (att.AttributeType == typeof(Test))
            {
                return true;
            }
        }

        return false;
    }
}